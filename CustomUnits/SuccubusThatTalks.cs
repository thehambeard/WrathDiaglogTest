using DialogTest.SimpleDialogs;
using DialogTest.Utilities;
using Kingmaker.Blueprints;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.DialogSystem;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic.Interaction;
using Kingmaker.View.Spawners;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using static DialogTest.Utilities.Loggers;

namespace DialogTest.CustomUnits
{
    internal class SuccubusThatTalks : IAreaLoadingStagesHandler, IDisposable
    {
        public readonly string Name = "SuccubusThatTalks";
        public readonly string Guid = "7FF65275-8CFF-41CD-99A8-50D554B1C554";
        public readonly string UnitTemplateGuid = "0d88d5c310fac90449bfd0714bb9f810";
        public readonly string ActionName = "ActionSuccubusThatTalks";
        public readonly string ActionGuid = "6FEB5447-E0B6-4EFD-8546-3AC213E1E2FF";
        public readonly string ActionsTemplate = "63a4a91f33825f34baf4a99b7989feff";
        public readonly string SpawnerPath = "[cr21] NocticulaPriestessPack/Spawner [CR11_SuccubusRanger] (pool) (3)";
        public readonly string SpawnerScene = "MidnightFane_Caves_Mechanics";

        public BlueprintUnit Blueprint { get; private set; }
        public ActionsHolder ActionsHolder { get; private set; }
        public BlueprintDialogReference Dialog { get; private set; }
        public UnitSpawner Spawner { get; private set; }
        public SpawnerInteractionActions SpawnerInteractionActions { get; private set; }
        public bool IsTargetScene => SceneManager.GetSceneByName(SpawnerScene).isLoaded;
        public UnitEntityData Unit { get; private set; }

        public void Create()
        {
            using (new ProcessLog("Unit creation time"))
            {
                Log($"Creating {Name}...");

                Log($"Fetching template blueprint {UnitTemplateGuid}...");
                var template = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>(UnitTemplateGuid);

                if (template == null)
                {
                    Log($"Fetch failed: {UnitTemplateGuid}");
                    return;
                }

                Blueprint = Helpers.CopyAndAdd(template, Name, Guid);
                Blueprint.m_DisplayName = GameStrings.CreateString($"dialogtest.unit.{Name}", "Chatty Succubus...");

                CreateDialog();
                CreateActionHolder();

                EventBus.Subscribe(this);
                Log($"{Name} creation finished...");
            }
        }

        private void CreateDialog()
        {
            Log($"Creating dialog for {Name}...");
            var bref = Blueprint.ToReference<BlueprintUnitReference>();

            Dialog = SimpleDialogBuilder.CreateDialog(
                name: "simpledialog.succubus.base",
                guid: "CB805DEE-7C4F-4F6A-AF6A-3CC8100CAEE2",
                firstCue: new()
                {
                    SimpleDialogBuilder.CreateCue(
                        name: "simpledialog.succubus.greet",
                        guid: "BCE7D981-4578-4AF2-A48D-E86C439B66E0", // this guid is referenced below
                        speaker: bref,
                        text: "I am just some bum who had no dialog but I can speak now!",
                        answerList: new()
                        {
                            SimpleDialogBuilder.CreateAnswerList(
                                name : "simpledialog.succubus.greet.answerlist",
                                guid : "785607FF-2B69-44AC-B959-CBAD6BED959C",
                                answers: new()
                                {
                                    SimpleDialogBuilder.CreateAnswer(
                                        name: "simpledialog.succubus.greet.answer.good",
                                        guid : "9D4E451B-F6DF-4425-8345-94324D6DA455",
                                        text: "You sound great!",
                                        nextCues: new()
                                        {
                                            SimpleDialogBuilder.CreateCue(
                                                name : "simpledialog.succubus.loop.good",
                                                guid : "E82E6E36-3EC4-4EF5-A9C5-16E2F94529E1",
                                                speaker: bref,
                                                text:"Thanks, I am off to tell the world! Looks like you have more to say...",
                                                cueSelection: new()
                                                {
                                                    Cues = new List<BlueprintCueBaseReference>()
                                                    {
                                                        new BlueprintCueBaseReference()
                                                        {
                                                            deserializedGuid = "BCE7D981-4578-4AF2-A48D-E86C439B66E0".ToGUID() // loop back to the beginning.
                                                        }
                                                    },
                                                    Strategy = Strategy.First
                                                })
                                        }),
                                    SimpleDialogBuilder.CreateAnswer(
                                        name : "simpledialog.succubus.greet.answer.bad",
                                        guid : "C343C70D-C063-4B43-A131-F0065AC8466A",
                                        text: "I liked you better when you couldn't talk.",
                                        nextCues: new()
                                        {
                                            SimpleDialogBuilder.CreateCue(
                                                name: "simpledialog.succubus.loop.bad",
                                                guid: "253E4E65-CFCB-48B3-B7C3-44D738343DA4",
                                                speaker: bref,
                                                text: "You should go back and say something nice before I drown you in this pool.",
                                                cueSelection: new()
                                                {
                                                    Cues = new List<BlueprintCueBaseReference>()
                                                    {
                                                        new BlueprintCueBaseReference()
                                                        {
                                                            deserializedGuid = "BCE7D981-4578-4AF2-A48D-E86C439B66E0".ToGUID() // loop back to the beginning
                                                        }
                                                    },
                                                    Strategy = Strategy.First
                                                })
                                        }),
                                    SimpleDialogBuilder.CreateAnswer(
                                        name : "simpledialog.succubus.exit",
                                        guid : "6E2B3156-B621-4919-BCD5-12DDDAA68E1A",
                                        text: "I'm outta here nerd.",
                                        nextCues: new()
                                        {
                                            SimpleDialogBuilder.CreateCue(
                                                name: "simpledialog.succubus.exit.response",
                                                guid: "2F44C1D1-041D-4480-86DC-09C103F80627",
                                                speaker: bref,
                                                text: "Then go, see if I care."
                                            )
                                        })
                                })
                        })
            });

            if (Dialog == null)
                Log("Dialog creation failed...");
        }

        private void CreateActionHolder()
        {
            Log($"Creating ActionHolder for {Name}...");

            ActionsHolder = Helpers.CreateAndAddESO<ActionsHolder>(ActionName, ActionGuid);

            var startDialog = Helpers.CreateElement<StartDialog>(ActionsHolder);
            startDialog.m_Dialogue = Dialog;
            ActionsHolder.AddToElementsList(startDialog);

            ActionsHolder.Actions = new ActionList()
            {
                Actions = new GameAction[]
                {
                    startDialog
                }
            };
        }

        public void ReplaceWithCustom()
        {
            if (!IsTargetScene)
                return;

            Log($"Target scene {SpawnerScene} detected!");
            Log($"Fetching spawner: {SpawnerPath}");

            Spawner = SceneManager.GetSceneByName(SpawnerScene).GetRootGameObjects().Where(obj => obj.name == "Spawners").FirstOrDefault().transform.Find(SpawnerPath).GetComponent<UnitSpawner>();

            if (Spawner == null)
            {
                Log($"Spawner: {SpawnerPath} not found...");
                return;
            }

            Log($"Replacing target unit with {Name}...");

            Spawner.Blueprint = Blueprint;

            Log($"Fetching spawner interactions...");

            SpawnerInteractionActions = Spawner.gameObject.GetComponent<SpawnerInteractionActions>();

            if (SpawnerInteractionActions == null)
            {
                Log("Fetching spawner interactions has failed...");
                return;
            }

            SpawnerInteractionActions.Actions = ActionsHolder.ToReference<ActionsReference>();

            Log($"Spawning {Name}...");

            Unit = Spawner.ForceReSpawn();

            if (Unit == null)
                Log("Spawning failed...");
        }

        public void OnAreaScenesLoaded() { }

        public void OnAreaLoadingComplete()
        {
            ReplaceWithCustom();
        }

        public void Dispose()
        {
            EventBus.Unsubscribe(this);
        }
    }
}




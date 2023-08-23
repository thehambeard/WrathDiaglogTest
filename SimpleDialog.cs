//
// Hambeard's SimpleDialog classes for creating dialog for NPC's that have bark
// The only thing special that is needed is a publicized version of Assembly-CSharp_public
// Follow the Wiki: https://github.com/WittleWolfie/OwlcatModdingWiki/wiki/Publicize-Assemblies on how to accomplish generating your own. 
//

using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.DialogSystem;
using Kingmaker.DialogSystem.Blueprints;
using Kingmaker.ElementsSystem;
using Kingmaker.Localization;
using Kingmaker.ResourceLinks;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic.Interaction;
using Kingmaker.UnitLogic.Mechanics;
using System;
using System.Collections.Generic;
using System.Linq;
using static DialogTest.SimpleDialogBuilder;

namespace DialogTest
{
    internal class SimpleDialog
    {
        private BarkDestoryer BarkDestoryer;
        public BlueprintDialogReference Dialog { get; private set; }
        public SimpleDialogUnit DialogUnit { get; private set; }

        public SimpleDialog(SimpleDialogUnit unit, BlueprintDialogReference dialog)
        {
            DialogUnit = unit;
            Dialog = dialog;
        }

        // Should be executed at launch to setup the dialogs. If a bark blueprint is provided it will setup the startdialog through it otherwise it will just the DialogOnClick method.
        public void Init()
        {
            if (DialogUnit.BarkGUID != "")
                BarkDestoryer = new(DialogUnit, Dialog);
            else
            {
                var comp = new DialogOnClick()
                {
                    m_Dialog = Dialog,
                    Conditions = Empties.Conditions,
                    NoDialogActions = Empties.Actions,
                };

                DialogUnit.Unit.Components = DialogUnit.Unit.Components.AddItem(comp).ToArray();
            }
        }
    }

    //
    // This is the BlueprintUnit that will have a dialog added to. Note: This will affect every unit that uses the Blueprint, you will notice all the succubi in the area have the new dialog. You'll need to clone
    // them so they are unique if you don't want to change all of them. The contructor takes the guid of the BlueprintUnit that is to be changed and the optional guid of the ActionsHolder you wish to overwrite. Note:
    // ActionsHolders are not SimpleBlueprint's, they are ElementsScriptableObject's
    //

    internal class SimpleDialogUnit
    {
        public string GUID { get; private set; }
        public string BarkGUID { get; private set; }
        public BlueprintUnit Unit { get; private set; }
        public BlueprintUnitReference UnitReference { get; private set; }

        public SimpleDialogUnit(string guid, string barkGUID = "")
        {
            GUID = guid;
            Unit = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>(guid);
            UnitReference = Unit.ToReference<BlueprintUnitReference>();
            BarkGUID = barkGUID;
        }
    }

    //
    // This class controls creating the dialog tree along intializing each unit's tree. Initialize() should be called at launch after the BlueprintCache is loaded.
    // 
    internal static class SimpleDialogController
    {
        private static readonly SimpleDialogUnit citizen = new("7001e2a58c9e86e43b679eda8a59f12f");                                     // Just a citizen npc with no bark data. You can spawn him from the UMM Menu. They will spawn on top of you so you'll have to move.
        private static readonly SimpleDialogUnit succ = new("0d88d5c310fac90449bfd0714bb9f810", "63a4a91f33825f34baf4a99b7989feff");    // Succubi in the pool area, guid to the bark data as well.

        public static Dictionary<SimpleDialogUnit, SimpleDialog> Dialogs = new()
        {
            {citizen, new(citizen, CreateDialog("simpledialog.citizen.base", "CB805DEE-7C4F-4F6A-AF6A-3CC8100CAEE2", new()
                {
                    CreateCue("simpledialog.citizen.greet", "BCE7D981-4578-4AF2-A48D-E86C439B66E0", citizen.UnitReference, "I am just some bum who had no dialog but I can speak now!", new ()
                    {
                        CreateAnswerList("simpledialog.citizen.greet.answerlist", "785607FF-2B69-44AC-B959-CBAD6BED959C", new ()
                        {
                            CreateAnswer("simpledialog.citizen.greet.answer.good", "9D4E451B-F6DF-4425-8345-94324D6DA455", "You sound great!", new ()
                            {
                                CreateCue("simpledialog.citizen.exit.good", "E82E6E36-3EC4-4EF5-A9C5-16E2F94529E1", citizen.UnitReference, "Thanks, I am off to tell the world!", new())
                            }),
                            CreateAnswer("simpledialog.citizen.greet.answer.bad", "C343C70D-C063-4B43-A131-F0065AC8466A", "I liked you better when you couldn't talk.", new ()
                            {
                                CreateCue("simpledialog.citizen.exit.bad", "253E4E65-CFCB-48B3-B7C3-44D738343DA4", citizen.UnitReference, "Maybe I should drown you in the pool of water.", new())
                            })
                        })
                    })
                }))
            },
            {succ, new(succ, CreateDialog("simpledialog.succ.base", "70CCF083-9C37-4E14-BBF2-E66FFB0C816C", new()
                {
                    CreateCue("simpledialog.succ.greet", "88C55921-524A-427D-8451-39B2FC8C2E7C", succ.UnitReference, "Hello there! This is a test! How do you feel today?", new ()
                    {
                        CreateAnswerList("simpledialog.succ.greet.answerlist", "84671A44-7CBF-4680-A0A4-604EB9387FC6", new ()
                        {
                            CreateAnswer("simpledialog.succ.greet.answer.good", "2ABB5DF5-72E0-44B7-9208-6E9E5B0D9D9C", "Hi I am feeling great!", new ()
                            {
                                CreateCue("simpledialog.succ.exit.good", "73BDD0CD-1CC2-4E95-8E78-7C89357BB2C6", succ.UnitReference, "I am glad to hear that!", new())
                            }),
                            CreateAnswer("simpledialog.succ.greet.answer.bad", "93D45D94-485A-402F-947D-ABC3D3DDE760", "I feel like crap!", new ()
                            {
                                CreateCue("simpledialog.succ.exit.bad", "1A1F3585-723E-4B10-80DF-8D58F1AAD1E8", succ.UnitReference, "You'd feel better if you were sitting in the water", new())
                            })
                        })
                    })
                }))
            }
        };

        public static void Initialize()
        {
            foreach (var kvp in Dialogs)
                kvp.Value.Init();
        }
    }

    //
    // The class that replaces all the actions in the ActionsHolder bark data with a simple StartDialog() action. This class could be expanded to provide more advanced features like random dialogs
    // each time you start talking to the NPC.
    //

    internal class BarkDestoryer
    {
        public BarkDestoryer(SimpleDialogUnit unit, BlueprintDialogReference dialog)
        {
            var ah = ResourcesLibrary.TryGetScriptable<ActionsHolder>(unit.BarkGUID).Actions = new()
            {
                Actions = new GameAction[]
                {
                    new StartDialog()
                    {
                        m_Dialogue = dialog
                    }
                }
            };
        }
    }

    //
    // Calls the controller to initialize everthing after BlueprintsCache has been initialized.
    //
    internal class Initializations
    {
        [HarmonyPatch(typeof(BlueprintsCache))]
        static class BlueprintsCache_Patch
        {

            [HarmonyPriority(Priority.First)]
            [HarmonyPatch(nameof(BlueprintsCache.Init)), HarmonyPostfix]
            static void Postfix()
            {
                SimpleDialogController.Initialize();
            }
        }
    }

    //
    // Just some helpers to ensure there are no null objects in the dialog trees
    //
    internal static class Empties
    {
        public static readonly ActionList Actions = new() { Actions = new GameAction[0] };
        public static readonly ConditionsChecker Conditions = new() { Conditions = new Condition[0] };
        public static readonly ContextDiceValue DiceValue = new()
        {
            DiceType = DiceType.Zero,
            DiceCountValue = 0,
            BonusValue = 0
        };
        public static readonly LocalizedString String = new();
        public static readonly PrefabLink PrefabLink = new();
        public static readonly ShowCheck ShowCheck = new();
        public static readonly CueSelection CueSelection = new();
        public static readonly CharacterSelection CharacterSelection = new();
        public static readonly DialogSpeaker DialogSpeaker = new() { NoSpeaker = true };
    }

    //
    // This is main class that will assit in building a very simple dialog tree. Expand as you see fit.
    //
    internal class SimpleDialogBuilder
    {
        // This method must be called first to start building the dialog tree. The other methods will create the other parts of the tree where you need them. 

        public static BlueprintDialogReference CreateDialog(string name, string guid, List<BlueprintCueBaseReference> firstCue)
        {
            var dialog = Create<BlueprintDialog>(name, guid);
            dialog.FirstCue = new CueSelection()
            {
                Cues = firstCue,
                Strategy = Strategy.First
            };

            dialog.Conditions = Empties.Conditions;
            dialog.StartActions = Empties.Actions;
            dialog.FinishActions = Empties.Actions;
            dialog.ReplaceActions = Empties.Actions;

            return dialog.ToReference<BlueprintDialogReference>();
        }

        public static BlueprintAnswerBaseReference CreateAnswer(string name, string guid, string text, List<BlueprintCueBaseReference> nextCues)
        {
            var answer = Create<BlueprintAnswer>(name, guid);
            answer.Text = GameStrings.CreateString(name, text);
            answer.NextCue = new CueSelection()
            {
                Cues = nextCues,
                Strategy = Strategy.First
            };

            answer.ShowCheck = Empties.ShowCheck;
            answer.ShowConditions = Empties.Conditions;
            answer.SelectConditions = Empties.Conditions;
            answer.OnSelect = Empties.Actions;
            answer.FakeChecks = new CheckData[0];
            answer.CharacterSelection = Empties.CharacterSelection;

            return answer.ToReference<BlueprintAnswerBaseReference>();
        }

        public static BlueprintCueBaseReference CreateCue(string name, string guid, BlueprintUnitReference speaker, string text, List<BlueprintAnswerBaseReference> answerList)
        {
            var cue = Create<BlueprintCue>(name, guid);
            cue.Text = GameStrings.CreateString(name, text);
            cue.Speaker = new()
            {
                m_Blueprint = speaker,
                MoveCamera = true
            };

            cue.Answers = answerList;

            if (cue.Text is null)
                cue.Text = Empties.String;
            if (cue.Speaker is null)
                cue.Speaker = Empties.DialogSpeaker;
            if (cue.Answers is null)
                cue.Answers = new();

            cue.Conditions = Empties.Conditions;
            cue.m_Listener = Activator.CreateInstance<BlueprintUnitReference>();
            cue.OnShow = Empties.Actions;
            cue.OnStop = Empties.Actions;
            cue.Continue = Empties.CueSelection;

            return cue.ToReference<BlueprintCueBaseReference>();
        }

        public static BlueprintAnswerBaseReference CreateAnswerList(string name, string guid, List<BlueprintAnswerBaseReference> answers)
        {
            var answerList = Create<BlueprintAnswersList>(name, guid);
            answerList.Answers = answers;

            if (answerList.Answers is null)
                answerList.Answers = new();

            answerList.Conditions = Empties.Conditions;

            return answerList.ToReference<BlueprintAnswerBaseReference>();
        }

        //
        // Creates a new blueprint T and adds it to the games BlueprintCache. Each blueprint needs a unique AssetGuid. I just use the Tools->Create GUID tool included in Visual Studio
        // to generate the guid
        //

        private static T Create<T>(string name, string guid) where T : SimpleBlueprint, new()
        {
            T asset = new()
            {
                name = name,
                AssetGuid = guid.ToGUID()
            };

            ResourcesLibrary.BlueprintsCache.AddCachedBlueprint(guid.ToGUID(), asset);

            return asset;
        }
    }

    //
    // Needed to create text in the game. Each text entry must have its own unique key. Generally it is good practice to precede your key with the name of your mod to avoid
    // key collisons. You can use the key to fetch the string. Helpful if you want to localize your mods to different languages.
    //

    internal static class GameStrings
    {
        private static Dictionary<string, LocalizedString> Strings = new();
        internal static LocalizedString CreateString(string key, string value)
        {
            var localizedString = new LocalizedString() { m_Key = key };
            LocalizationManager.CurrentPack.PutString(key, value);
            Strings.Add(key, localizedString);
            return localizedString;
        }

        internal static LocalizedString GetString(string key)
        {
            return Strings.ContainsKey(key) ? Strings[key] : default;
        }
    }

    internal static class Extensions
    {
        public static BlueprintGuid ToGUID(this string guid)
        {
            return new BlueprintGuid(Guid.Parse(guid));
        }
    }
}

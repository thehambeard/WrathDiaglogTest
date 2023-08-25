using DialogTest.Utilities;
using Kingmaker.Blueprints;
using Kingmaker.DialogSystem;
using Kingmaker.DialogSystem.Blueprints;
using System;
using System.Collections.Generic;
using static DialogTest.Utilities.Helpers;

namespace DialogTest.SimpleDialogs
{
    internal class SimpleDialogBuilder
    {
        // This method must be called first to start building the dialog tree. The other methods will CreateAndAdd the other parts of the tree where you need them. 

        public static BlueprintDialogReference CreateDialog(string name, string guid, List<BlueprintCueBaseReference> firstCue)
        {
            var dialog = CreateAndAdd<BlueprintDialog>(name, guid);
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
            var answer = CreateAndAdd<BlueprintAnswer>(name, guid);
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
            var cue = CreateAndAdd<BlueprintCue>(name, guid);
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
            var answerList = CreateAndAdd<BlueprintAnswersList>(name, guid);
            answerList.Answers = answers;

            if (answerList.Answers is null)
                answerList.Answers = new();

            answerList.Conditions = Empties.Conditions;

            return answerList.ToReference<BlueprintAnswerBaseReference>();
        }
    }

}

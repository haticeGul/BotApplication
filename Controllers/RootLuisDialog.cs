namespace BotApplication
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Builder.FormFlow;
    using BotApplication;

    using static Constant;
    [LuisModel("90547653-8a29-48b0-907c-5e1bcc19db3f", "3f9616fe3cff456f9885a683011111c2")]
    [Serializable]
    public class RootLuisDialog : LuisDialog<object>
    {
        #region None
        //Eğer sorulan cümle anlaşılamazsa düşeceği default method. 
        [LuisIntent("")]
        [LuisIntent("None")]
       
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = "Üzgünüm, " + result.Query + " içeriğini tam olarak anlayamadım. Size nasıl yardımcı olacağımı 'help' veya 'help me' yazarak öğrenebilirsiniz.";
            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }
        #endregion

        #region Help 
        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Selam! Benden 'Teklif çıktısını almak istiyorum', 'Poliçelerime erişmek istiyorum' gibi taleplerde bulunabilirsiniz.");

            context.Wait(this.MessageReceived);
        }
        #endregion

        #region Show Policy
        

        [LuisIntent("CreateTask")]
        public async Task CreateTask(IDialogContext context, LuisResult result)
        {
            var options = new Selection[] { Selection.Insurance, Selection.Credit };
            var descriptions = new string[] {"Sigorta Talebi", "Kredi Talebi" };

            PromptDialog.Choice<Selection>(context, ResumeChooseCategory, options, "Hangi kategoride talep açmak istediğinizi belirtiniz.", descriptions: descriptions);
        }
        
        private async Task ResumeChooseCategory(IDialogContext context, IAwaitable<Selection> result)
        {
            var selection = await result;

            context.ConversationData.SetValue("Category", selection);

            SelectionSubject[] options;
            string[] descriptions;
            if (selection == Selection.Insurance)
            {
                options = new SelectionSubject[] { SelectionSubject.CancelPolicy, SelectionSubject.CreatePolicy, SelectionSubject.CollectionRequest };
                descriptions = new string[] { "Kasko iptal/iade Talebi", "Kasko/Trafik Poliçe Talebi", "Tahsilat Bilgisi Talebi" };
            }
            else
            {
                options = new SelectionSubject[] { SelectionSubject.EarlyClose, SelectionSubject.DeleteHypotec, SelectionSubject.SendMailNoDebt };
                descriptions = new string[] { "Erken Kapama", "Rehin Kaldırma", "Borcu yoktur emaili gönderme" };
            }
            
            PromptDialog.Choice<SelectionSubject>(context, ResumeChooseTaskSubject, options,"Talep Kategorisini seçiniz.", descriptions: descriptions);
                       
        }
        
        private async Task ResumeChooseTaskSubject(IDialogContext context, IAwaitable<SelectionSubject> result)
        {
            var tasksubject = await result;
            context.ConversationData.SetValue("TaskSubject", tasksubject);
            PromptDialog.Text(context, ResumeTakeExplanation, "Lütfen talep açıklaması giriniz.");
        }
        private async Task ResumeTakeExplanation(IDialogContext context, IAwaitable<string> result)
        {
            var explanation = await result;

            context.ConversationData.SetValue("Explanation", explanation);

            PromptDialog.Text(context, ResumeTakePhone, "İletişim bilgilerinizi giriniz.");
        }
        private async Task ResumeTakePhone(IDialogContext context, IAwaitable<string> result)
        {
            var communicationInfo = await result;

            string message = "Girmiş olduğunuz taleple ilgili sizinle en yakın zamanda iletişime geçilecektir.";
            var x = context.ConversationData;

            var category = x.GetValue<string>("Category");
            var taskSubject = x.GetValue<string>("TaskSubject");
            var selection = x.GetValue<string>("Explanation");

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }
        #endregion

    }
}

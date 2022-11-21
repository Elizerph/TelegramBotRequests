namespace TelegramBotTest.Components
{
    public class TicketTemplate
    {
        public static TicketTemplate Default { get; } = new()
        {
            NewTicketTitle = "<emo1F4C4> Новая заявка",
            AcceptedTicketTitle = "<emo1F552> Заявка в работе",
            DoneTicketTitle = "<emo2705> Заявка выполнена",
            DropTicketTitle = "<emo274C> Заявка отклонена",
            CompletedMessage = "Заявка создана",
            AcceptButtonLabel = "<emo1F446> Принять",
            DoneButtonLabel = "<emo2705> Выполнено",
            DropButtonLabel = "<emo274C> Отклонено",
            NewTicketHistory = "<time>: <user> создано",
            AcceptedTicketHistory = "<time>: <user> принято",
            DoneTicketHistory = "<time>: <user> выполнено",
            DropTicketHistory = "<time>: <user> отклонено",
            FieldNames = new[]
            {
                "Тема",
                "Адрес",
                "Время"
            }
        };

        public string NewTicketTitle { get; set; }
        public string AcceptedTicketTitle { get; set; }
        public string DoneTicketTitle { get; set; }
        public string DropTicketTitle { get; set; }
        public string CompletedMessage { get; set; }
        public string AcceptButtonLabel { get; set; }
        public string DoneButtonLabel { get; set; }
        public string DropButtonLabel { get; set; }
        public string NewTicketHistory { get; set; }
        public string AcceptedTicketHistory { get; set; }
        public string DoneTicketHistory { get; set; }
        public string DropTicketHistory { get; set; }
        public string[] FieldNames { get; set; }
    }
}

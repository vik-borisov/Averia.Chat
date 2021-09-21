namespace Averia.Chat.Entities
{
    /// <summary>
    /// Сообщение
    /// </summary>
    public class Message
    {
        
        /// <summary>
        /// Дата создания
        /// </summary>
        public long CreateDate { get; set; }
        
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public string UserId { get; set; }
        
        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string Text { get; set; }
    }
}
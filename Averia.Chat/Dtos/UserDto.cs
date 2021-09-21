namespace Averia.Chat.Dtos
{
    /// <summary>
    /// Статус пользователя
    /// </summary>
    public enum EUserStatus
    {
        /// <summary>
        /// Присоеденился
        /// </summary>
        Join = 10,
        
        /// <summary>
        /// Отсоеденился
        /// </summary>
        Left = 20
    } 
    
    /// <summary>
    /// ДТО пользовталея
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public string UserId { get; set; }
        
        /// <summary>
        /// Статус пользователя
        /// </summary>
        public EUserStatus Status { get; set; }
    }
}
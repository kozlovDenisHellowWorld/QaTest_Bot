using System;
using System.ComponentModel.DataAnnotations;

namespace Web_test_bot.DbTools.DbObjeckts
{
    public interface IDbMyElement
    {
        /// <summary>
        /// Индификатор в базе данных
        /// </summary>
        [Key]
        public int Id { get; set; } // Идентификатор чата

        /// <summary>
        /// Полное название объекта
        /// </summary>
        public string EntyName { get; } // Русское название элемента

        /// <summary>
        /// Код элемента объекта
        /// </summary>
        public string EntyCode { get; } // Код элемекнта для использования в call back

        /// <summary>
        /// Дата зоздания объекта
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Удален ли объект
        /// </summary>
        public bool? IsDelite { get; set; }
    }
}

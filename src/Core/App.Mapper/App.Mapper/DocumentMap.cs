using System.Text.Json;
using App.Contracts.Models;
using App.Domain.Entities;

namespace App.Mapper
{
    public static class DocumentMap
    {
        /// <summary>
        /// Convert Document DTO model to entity
        /// </summary>
        /// <param name="documentDto">DTO model</param>
        /// <returns>final entity</returns>
        public static Document ToEntity(this DocumentDto documentDto)
        {
            return new Document { 
                Id = documentDto.id,
                Value = JsonSerializer.Serialize(documentDto) 
            };
        }

        /// <summary>
        /// Convert Document entity to DTO model
        /// </summary>
        /// <param name="document">document entity</param>
        /// <returns>final DTO model</returns>
        public static DocumentDto ToDtoModel(this Document document)
        {
            return JsonSerializer.Deserialize<DocumentDto>(document.Value);
        }
    }
}

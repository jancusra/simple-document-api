using System.Text.Json;
using App.Contracts.Models;
using App.Domain.Entities;

namespace App.Mapper
{
    public static class DocumentMap
    {
        public static Document ToEntity(this DocumentDto documentDto)
        {
            return new Document { 
                Id = documentDto.id,
                Value = JsonSerializer.Serialize(documentDto) 
            };
        }

        public static DocumentDto ToDtoModel(this Document document)
        {
            return JsonSerializer.Deserialize<DocumentDto>(document.Value);
        }
    }
}

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TodoListAPI
{
    public class TodoItem
    {
        [Key] public int Id { get; set; }
        [Required] public required string Title { get; set; }
        [Required] public Status Status { get; set; } = Status.pending;
        [Required] public required string UserName { get; set; }
    }


    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Status
    {
        pending,
        completed
    }
}
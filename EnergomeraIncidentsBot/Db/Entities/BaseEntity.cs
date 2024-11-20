using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace EnergomeraIncidentsBot.Db.Entities;

/// <summary>
/// Базовый класс сущности.
/// </summary>
/// <typeparam name="TKey"></typeparam>
public class BaseEntity<TKey> : IBaseEntity
{
    /// <summary>
    /// Идентификатор сущности.
    /// </summary>
    [Comment("ИД.")]
    public TKey Id { get; set; }
    
    /// <inheritdoc />
    [Comment("Когда создана сущность.")]
    [JsonIgnore]
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <inheritdoc />
    [Comment("Кем создана сущность.")]
    [JsonIgnore]
    public string? CreatedBy { get; set; }
    
    /// <inheritdoc />
    [Comment("Когда обновлена сушность.")]
    [JsonIgnore]
    public DateTimeOffset? UpdatedAt { get; set; }
    
    /// <inheritdoc />
    [Comment("Кем обновлена сущность.")]
    [JsonIgnore]
    public string? UpdatedBy { get; set; }
    
    /// <inheritdoc />
    [Comment("Когда удалена сущность.")]
    [JsonIgnore]
    public DateTimeOffset? DeletedAt { get; set; }
    
    /// <inheritdoc />
    [Comment("Кем удалена сущность.")]
    [JsonIgnore]
    public string? DeletedBy { get; set; }
}
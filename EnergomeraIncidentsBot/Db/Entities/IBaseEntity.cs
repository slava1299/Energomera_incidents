namespace EnergomeraIncidentsBot.Db.Entities;

/// <summary>
/// Поля для базовой сущности.
/// </summary>
public interface IBaseEntity
{
    /// <summary>
    /// Когда создана сущность.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// Кем создана сущность.
    /// </summary>
    public string? CreatedBy { get; set; }
    
    /// <summary>
    /// Когда обновлена сущность.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }
    
    /// <summary>
    /// Кем обновлена сущность.
    /// </summary>
    public string? UpdatedBy { get; set; }
    
    /// <summary>
    /// Когда удалена сущность.
    /// </summary>
    public DateTimeOffset? DeletedAt { get; set; }
    
    /// <summary>
    /// Кем удалена сущность.
    /// </summary>
    public string? DeletedBy { get; set; }
    
}
namespace LevelUp.Microservice.Auth.Db.Entities;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public bool IsSent { get; set; }
    public string Payload { get; set; }
}
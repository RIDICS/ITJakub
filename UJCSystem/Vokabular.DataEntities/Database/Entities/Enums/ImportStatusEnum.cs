namespace Vokabular.DataEntities.Database.Entities.Enums
{
    public enum ImportStatusEnum : byte
    {
        Completed = 0,
        Failed = 1,
        CompletedWithWarnings = 2,
        Running = 3,
    }
}

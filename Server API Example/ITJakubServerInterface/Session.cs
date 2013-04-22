namespace ITJakubServerInterface
{
    //��dek tabulky pro relace
    //Prom�nn� Id je ve v�ech t��d�ch pro intern� pot�ebu datab�ze a nem��e se m�nit v k�du
    public class Session
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public long SessionUpdateId { get; set; }

        public long PrefferedUserId { get; set; }

        public long OwnerUserId { get; set; }
    }
}
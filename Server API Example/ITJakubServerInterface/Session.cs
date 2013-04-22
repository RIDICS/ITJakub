namespace ITJakubServerInterface
{
    //Øádek tabulky pro relace
    //Promìnná Id je ve všech tøídách pro interní potøebu databáze a nemùže se mìnit v kódu
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
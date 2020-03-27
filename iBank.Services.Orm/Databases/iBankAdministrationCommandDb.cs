namespace iBank.Services.Orm.Databases
{
    public class iBankAdministrationCommandDb : AbstractCommandDb
    {
        public iBankAdministrationCommandDb()
        {
            Context = new iBankAdministrationEntities();
        }
        public override object Clone()
        {
            return new iBankAdministrationCommandDb();
        }
    }
}

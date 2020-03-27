namespace iBank.Services.Orm.Databases
{
    public class iBankMastersCommandDb : AbstractCommandDb
    {
        public iBankMastersCommandDb()
        {
            Context = new iBankMastersEntities();
        }

        public override object Clone()
        {
            return new iBankMastersCommandDb();
        }
    }
}

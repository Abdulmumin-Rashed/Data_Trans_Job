namespace Data_Trans_Job.IService.IRepositories
{
    public interface IUnitOfWork
    {
      IAdminRepository Admin { get; }
      IPostRepository Post { get; }
      IRoleRepository Role { get; }
      void Save();
    }
}

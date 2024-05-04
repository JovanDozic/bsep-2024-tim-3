namespace Marketing_system.BL.Contracts
{
    public class PagedResult<TEntity> where TEntity : class
    {
        public List<TEntity> Result { get; set; }
        public int Count { get; set; }
    }
}

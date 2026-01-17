namespace AuthenticationAPI.Data.TableRowModels
{
    public class ValidUserRoleTableRow
    {
        public Guid Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public DateTime RowAddedDt { get; set; }
    }
}

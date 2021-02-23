namespace AlgeibaGoAPI.Models
{
    public partial class RelRouteUser
    {
        public int IdRelation { get; set; }
        public int? IdRoute { get; set; }
        public string IdUser { get; set; }

        public Routes IdRouteNavigation { get; set; }
        public AspNetUsers IdUserNavigation { get; set; }
    }
}

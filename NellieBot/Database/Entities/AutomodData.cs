using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NellieBot.Database.Entities
{
  public class AutomodData
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Label { get; set; }
    public List<string> Words { get; set; }
    public List<string> Regexes { get; set; }
    public string Alert { get; set; }
  }
}

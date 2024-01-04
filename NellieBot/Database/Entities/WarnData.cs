using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NellieBot.Database.Entities
{
  [PrimaryKey(nameof(Id), nameof(UserId))]
  public class WarnData
  {
    public int Id { get; set; }
    public ulong UserId { get; set; }
    public string Reason { get; set; }
    public string Note { get; set; }
    public DateTime DateTime { get; set; }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace ai_tool_suite.Models
{
    public class Users
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Teams { get; set; }

    }

    public class Prompts
    {
        public int ID { get; set; }
        public string Prompt { get; set; }
        public string Result { get; set; } = "";
        public int PromptTokens { get; set; } = 0;
        public int CompletionTokens { get; set; } = 0;
        public int TotalTokens { get; set; } = 0;
    }

    public class Pdfs
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
    }

    public class UserContext : DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<Prompts> Prompts { get; set; }

        public DbSet<Pdfs> Pdfs { get; set; }
    }

}
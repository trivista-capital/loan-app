using System;
using Microsoft.AspNetCore.Identity;

namespace Trivista.LoanApp.ApplicationCore.Entities;

public sealed class ApplicationRole
{
    internal ApplicationRole() { }

    private ApplicationRole(Guid id, string name, string description)
    {
        Id = id;
        Description = description;
        CreatedOn = DateTime.UtcNow;
        LastModified = DateTime.UtcNow;
        this.Name = name;
    }
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime LastModified { get; set; }

    public string? LastModifiedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public bool IsDeleted { get; set; }

   
    public class Factory
    {
        public static ApplicationRole Create(Guid id, string name, string description)
        {
            return new ApplicationRole(id, name, description);
        }

        public static ApplicationRole Create()
        {
            return new ApplicationRole();
        }
    }


    public ApplicationRole SetId(Guid id)
    {
        this.Id = id;
        return this;
    }

    public ApplicationRole SetLastModified()
    {
        this.LastModified = DateTime.UtcNow;
        return this;
    }    
}


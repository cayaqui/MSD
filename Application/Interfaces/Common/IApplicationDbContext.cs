using Domain.Entities.Projects;
using Domain.Entities.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Common
{

    public interface IApplicationDbContext
    {
        // Security
        DbSet<User> Users { get; }
        DbSet<ProjectTeamMember> ProjectTeamMembers { get; }

        // Setup
        DbSet<Company> Companies { get; }
        DbSet<Operation> Operations { get; }
        DbSet<Project> Projects { get; }
        
        //DbSet<Contractor> Contractors { get; }
        //DbSet<Discipline> Disciplines { get; }
        //DbSet<Phase> Phases { get; }
        //DbSet<WorkPackage> WorkPackages { get; }
        //DbSet<Package> Packages { get; }
        //DbSet<PackageDiscipline> PackageDisciplines { get; }

        // UI
        DbSet<Notification> Notifications { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }

}

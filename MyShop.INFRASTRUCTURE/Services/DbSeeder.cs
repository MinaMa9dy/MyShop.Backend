using MyShop.CORE.Entities;
using MyShop.INFRASTRUCTURE.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyShop.INFRASTRUCTURE.Services
{
    public static class DbSeeder
    {
        public static async Task SeedAttributesAsync(AppDbContext context)
        {
            if (!context.Attributes.Any())
            {
                var attributes = new List<CORE.Entities.Attribute>
                {
                    new CORE.Entities.Attribute { Id = Guid.NewGuid(), Name = "اللون", DataType = "Color" },
                    new CORE.Entities.Attribute { Id = Guid.NewGuid(), Name = "المقاس", DataType = "Size" },
                    new CORE.Entities.Attribute { Id = Guid.NewGuid(), Name = "الوزن", DataType = "Weight" },
                    new CORE.Entities.Attribute { Id = Guid.NewGuid(), Name = "النوع", DataType = "Type" }
                };

                await context.Attributes.AddRangeAsync(attributes);
                await context.SaveChangesAsync();
            }
            else
            {
                // Fix existing question marks if any
                var corruptedAttributes = context.Attributes.Where(a => a.Name.Contains("?")).ToList();
                if (corruptedAttributes.Any())
                {
                    foreach (var attr in corruptedAttributes)
                    {
                        // Logic to guess or reset corrupted names
                        // Since we don't know which is which, we can only reset if we have a pattern
                        // For now, let's just log or provide a manual fix script
                    }
                }
            }
        }
    }
}

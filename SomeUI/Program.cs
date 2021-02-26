using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeUI
{
    class Program
    {
        private static SamuraiContext _context = new SamuraiContext();
        static void Main(string[] args)
        {
            //PrePopulateSamuraisAndBattles();
            //JoinBattleAndSamurai();
            // EnlistSamuraiIntoABattle();
            //EnlistSamuraiIntoABattleUntracked();
            //AddNewSamuraiViaDisconnectedBattleObject();
            // AddNewSamuraiBattles();
            // AddNewSamuraiBattleswithAttachSamurai();
            // RemoveBattleFromSamurai();
            //RemoveBattleFromSamuraiWithAttachWhenDisconnected();
            // AddNewSamuraiWithSecretIdentity();
            //  AddSecretIdentityUsingSamuraiId();
            //AddSecretIdentityToExistingSamurai();
            //EditASecretIdentity();
            //ReplaceASecretIdentity();
            //CreateSamurai();
            //  CreateThenEditSamuraiWithQuote();
            // GetAllSamurais();
            // CreateSamuraiWithBetterName();
            //RetrieveAndUpdateBetterName();
            //ReplaceBetterName();
            // CreateAndFixUpNullBetterName();
            //CreateSamuraiWithBetterName();
            GetStats();
            Filter();
            Project();
        }
        private static void GetStats()
        {
            var stats = _context.SamuraiBattleStats.AsNoTracking().ToList();
        }
        private static void Filter()
        {
            var stats = _context.SamuraiBattleStats.Where(s => s.SamuraiId == 2).AsNoTracking().ToList();
        }
        private static void Project()
        {
            var stats = _context.SamuraiBattleStats.AsNoTracking().Select(s => new { s.Name, s.NumberOfBattles }).ToList();
        }
        private static void CreateSamuraiWithBetterName()
        {
            var samurai = new Samurai
            {
                Name = "Jack le Black",
                BetterName = PersonFullName.Create("Jack", "Black")
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }
        private static void CreateAndFixUpNullBetterName()
        {
            //_context.Samurais.Add(new Samurai { Name = "Chrisjen" });
            //_context.SaveChanges();
            using (_context = new SamuraiContext())
            {
                var persistedSamurai = _context.Samurais.FirstOrDefault(s => s.Name == "Chrisjen");
                if (persistedSamurai is null) { return; }
                _context.Entry(persistedSamurai).Reference(s => s.BetterName).TargetEntry.State
                  = EntityState.Detached;
                if (persistedSamurai.BetterName.IsEmpty())
                {
                    persistedSamurai.BetterName = null;
                }
              
                _context.Samurais.Update(persistedSamurai);
                _context.SaveChanges();
            }   
           
          
        }
        private static void ReplaceBetterName()
        {
            var samurai = _context.Samurais.FirstOrDefault(s => s.Id == 4);
            //_context.Entry(samurai).Reference(s => s.BetterName)
            //    .TargetEntry.State = EntityState.Detached;
            samurai.BetterName = PersonFullName.Create("Shohreh36459", "Aghdashloo");
            _context.Samurais.Update(samurai);
            DisplayStates(_context.ChangeTracker.Entries());
            _context.SaveChanges();
        }

        //private static void RetrieveAndUpdateBetterName()
        //{
        //    var samurai = _context.Samurais.FirstOrDefault(s => s.BetterName.SurName == "Black");
        //    samurai.BetterName.GivenName = "Jill";
        //    _context.SaveChanges();
        //}
        //private static void CreateSamuraiWithBetterName()
        //{
        //    var samurai = new Samurai
        //    {
        //        Name = "Jack le Black",
        //        BetterName = new PersonFullName("Jack", "Black")
        //    };
        //    _context.Samurais.Add(samurai);
        //    _context.SaveChanges();
        //}
        private static void GetAllSamurais()
        {
            var allsamurais = _context.Samurais.ToList();
        }
        private static void CreateThenEditSamuraiWithQuote()
        {
            var samurai = new Samurai { Name = "Ronin" };
            var quote = new Quote { Text = "Aren't I MARVELous?" };
            samurai.Quotes.Add(quote);
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
            quote.Text += " See what I did there?";
            _context.SaveChanges();
        }
        private static void CreateSamurai()
        {
            var samurai = new Samurai { Name = "Ronin" };
            _context.Samurais.Add(samurai);
            _context.Entry(samurai).Property("Created").CurrentValue = DateTime.Now;
            _context.Entry(samurai).Property("LastModified").CurrentValue = DateTime.Now;
            _context.SaveChanges();
        }
        private static void ReplaceASecretIdentity()
        {
            Samurai samurai;
            using (var separateOperation = new SamuraiContext())
            {
                samurai = separateOperation.Samurais.Include(s => s.SecretIdentity)
                                           .FirstOrDefault(s => s.Id == 1);
            }
            samurai.SecretIdentity. RealName = "T'Challa";
            _context.Samurais.Update(samurai);
            _context.Samurais.Attach(samurai);
           

            DisplayStates(_context.ChangeTracker.Entries());
            //this will fail...EF Core tries to insert a duplicate samuraiID FK
           
            _context.SaveChanges();

        }

        private static void EditASecretIdentity()
        {
            var samurai = _context.Samurais.Include(s => s.SecretIdentity)
                                  .FirstOrDefault(s => s.Id == 1);
            samurai.SecretIdentity.RealName = "T'Challa";
            _context.Samurais.Attach(samurai);
            DisplayStates(_context.ChangeTracker.Entries());
            _context.SaveChanges();
        }
        private static void AddSecretIdentityToExistingSamurai()
        {
            Samurai samurai=  _context.Samurais.Find(3); ;
            //using (var separateOperation = new SamuraiContext())
            //{
            //    samurai = _context.Samurais.Find(1);
            //}
            samurai.SecretIdentity = new SecretIdentity { RealName = "Julia1" };
            _context.Samurais.Attach(samurai);
          
            _context.SaveChanges();
        }
        private static void AddSecretIdentityUsingSamuraiId()
        {
            //Note: SamuraiId 1 does not have a secret identity yet!
            var identity = new SecretIdentity { SamuraiId = 1, };
            _context.Add(identity);
            _context.SaveChanges();
        }
        private static void AddNewSamuraiWithSecretIdentity()
        {
            var samurai = new Samurai { Name = "Jina Ujichika" };
            samurai.SecretIdentity = new SecretIdentity { RealName = "Julie" };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }
        private static void RemoveBattleFromSamuraiWithAttachWhenDisconnected()
        {
            //Goal:Remove join between Shichirōji(Id=3) and Battle of Okehazama (Id=1)
            var samurai = _context.Samurais.Include(s => s.SamuraiBattles)
                                           .ThenInclude(sb => sb.Battle)
                                  .SingleOrDefault(s => s.Id == 3);
            var sbToRemove = samurai.SamuraiBattles.SingleOrDefault(sb => sb.BattleId == 1);
          
            //_context.Remove(sbToRemove); //remove using DbContext
            _context.Attach(samurai);
            DisplayStates(_context.ChangeTracker.Entries());
            _context.ChangeTracker.DetectChanges(); //here for debugging
            samurai.SamuraiBattles.Remove(sbToRemove); //remove via List<T>
            _context.SaveChanges();
        }
        private static void RemoveBattleFromSamurai()
        {
            //Goal:Remove join between Shichirōji(Id=3) and Battle of Okehazama (Id=1)
            var samurai = _context.Samurais.Include(s => s.SamuraiBattles)
                                           .ThenInclude(sb => sb.Battle)
                                  .SingleOrDefault(s => s.Id == 3);
            var sbToRemove = samurai.SamuraiBattles.SingleOrDefault(sb => sb.BattleId == 1);
            //samurai.SamuraiBattles.Remove(sbToRemove); //remove via List<T>
            _context.Remove(sbToRemove); //remove using DbContext
            DisplayStates(_context.ChangeTracker.Entries());
            _context.ChangeTracker.DetectChanges(); //here for debugging
          
            _context.SaveChanges();
        }
        private static void GetSamuraiWithBattles()
        {
            var samuraiWithBattles = _context.Samurais
                .Include(s => s.SamuraiBattles)
                .ThenInclude(sb => sb.Battle).FirstOrDefault(s => s.Id == 1);
            var battle = samuraiWithBattles.SamuraiBattles.First().Battle;
            var allTheBattles = new List<Battle>();
            foreach (var samuraiBattle in samuraiWithBattles.SamuraiBattles)
            {
                allTheBattles.Add(samuraiBattle.Battle);
            }
        }
        private static void AddNewSamuraiViaDisconnectedBattleObject()
        {
            Battle battle;
            battle = _context.Battles.Find(1);
            //using (var separateOperation = new SamuraiContext())
            //{
            //    battle = separateOperation.Battles.Find(1);
            //}
            var newSamurai = new Samurai { Name = "SampsonSan22" };
            battle.SamuraiBattles.Add(new SamuraiBattle { Samurai = newSamurai });

            _context.Battles.Attach(battle);
            _context.ChangeTracker.DetectChanges();
            _context.SaveChanges();
        }

        private static void AddNewSamuraiBattles()
        {

            Battle battle = new Battle
            {
                Name = "Boshin War1 mohamed"
            };
            Samurai samurai = new Samurai
            {
                Name= "mohamed"
            };

            var samuraiBattle = new SamuraiBattle { Battle = battle, Samurai = samurai };
            battle.SamuraiBattles.Add(samuraiBattle);
            _context.Attach(battle);
            DisplayStates(_context.ChangeTracker.Entries());
            _context.SaveChanges();
        }
        private static void AddNewSamuraiBattleswithAttachSamurai()
        {

            Battle battle = new Battle
            {
                Name = "Boshin War1 mohamed2"
            };
            Samurai samurai = new Samurai
            {
                Name = "mohamed2"
            };

            var samuraiBattle = new SamuraiBattle { Battle = battle, Samurai = samurai };
            battle.SamuraiBattles.Add(samuraiBattle);
            _context.Battles.Attach(battle);
            DisplayStates(_context.ChangeTracker.Entries());
            _context.SaveChanges();
        }
        private static void EnlistSamuraiIntoABattleUntracked()
        {
            Battle battle;
            battle = _context.Battles.Find(1);
            //using (var separateOperation = new SamuraiContext())
            //{
            //    battle = separateOperation.Battles.Find(1);
            //}
            battle.SamuraiBattles.Add(new SamuraiBattle { SamuraiId = 3 });
            _context.Battles.Attach(battle);
            _context.ChangeTracker.DetectChanges(); //here to show you debugging info
            DisplayStates(_context.ChangeTracker.Entries());
            _context.SaveChanges();

        }
        private static void EnlistSamuraiIntoABattle()
        {
            var battle = _context.Battles.Find(1);

            battle.SamuraiBattles
                .Add(new SamuraiBattle { SamuraiId = 2 });
            _context.SaveChanges();
        }
        private static void JoinBattleAndSamurai()
        {
            //Kikuchiyo id is 1, Siege of Osaka id is 3
            var sbJoin = new SamuraiBattle { SamuraiId = 1, BattleId = 3 };
            _context.Add(sbJoin);
            _context.SaveChanges();
        }
        private static void PrePopulateSamuraisAndBattles()
        {
            _context.AddRange(
             new Samurai { Name = "Kikuchiyo" },
             new Samurai { Name = "Kambei Shimada" },
             new Samurai { Name = "Shichirōji " },
             new Samurai { Name = "Katsushirō Okamoto" },
             new Samurai { Name = "Heihachi Hayashida" },
             new Samurai { Name = "Kyūzō" },
             new Samurai { Name = "Gorōbei Katayama" }
           );

            _context.Battles.AddRange(
             new Battle { Name = "Battle of Okehazama", StartDate = new DateTime(1560, 05, 01), EndDate = new DateTime(1560, 06, 15) },
             new Battle { Name = "Battle of Shiroyama", StartDate = new DateTime(1877, 9, 24), EndDate = new DateTime(1877, 9, 24) },
             new Battle { Name = "Siege of Osaka", StartDate = new DateTime(1614, 1, 1), EndDate = new DateTime(1615, 12, 31) },
             new Battle { Name = "Boshin War", StartDate = new DateTime(1868, 1, 1), EndDate = new DateTime(1869, 1, 1) }
           );
            _context.SaveChanges();
        }

        private static void DisplayStates(IEnumerable<EntityEntry> entries)
        {
            foreach (var entry in entries)
            {
                Console.WriteLine($"Entity: {entry.Entity.GetType().Name},State: { entry.State.ToString()}");
            }
        }
    }
}

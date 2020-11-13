// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SillyVmo.cs" company="The Silly Company">
//   The Silly Company 2016. All rights reserved.
// </copyright>
// <summary>
//   The silly vmo.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows.Input;

using DragAndDropSample.Services;

namespace DragAndDropSample.ViewModels
{
    public class SillyDudeVmo
    {
        public SillyDudeVmo(SillyDude dude, ICommand tapCommand)
        {
            if (dude != null)
            {
                Id = dude.Id;
                Name = dude.Name;
                FullName = dude.FullName;
                Role = dude.Role;
                Description = dude.Description;
                ImageUrl = dude.ImageUrl;
                SillinessDegree = dude.SillinessDegree;
                SourceUrl = dude.SourceUrl;
            }

            TapCommand = tapCommand;
        }

        public bool IsMovable { get; protected set; } = true;

        public ICommand TapCommand { get; set; }

        public int Id { get; }

        public string Name { get; }

        public string FullName { get; }

        public string Role { get; }

        public string Description { get; }

        public string ImageUrl { get; }

        public int SillinessDegree { get; }

        public string SourceUrl { get; }

        public void Lock()
        {
            IsMovable = false;
        }
    }

    public class AddSillyDudeVmo : SillyDudeVmo
    {
        public AddSillyDudeVmo(ICommand tapCommand)
            : base(null, tapCommand)
        {
        }
    }
}
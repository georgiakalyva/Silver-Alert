using NotificationsExtensions.TileContent;
using SilverAlert.Shared;
using System;
using System.Linq;
using System.Collections.Generic;
using Windows.UI.Notifications;

namespace SilverAlert.WindowsStore.EnableLiveTile
{
    public class CreateLiveTile
    {
        public static async void ShowliveTile()
        {
            try
            {
                string FileData = await FileManagement.ReadFile("MissingPeople.json");
                
                string ResultJson = "[" + FileData.ToString() + "]";

                List<MissingPerson> Mis = JsonData.MissingPeopleList("el", ResultJson, Category.Missing).OrderByDescending(x => x.DateMissing).ToList<MissingPerson>();

                if (Mis.Count >= 5)
                {
                    string SkippedItemsString = AppStorage.SkippedItems.Get();

                    String[] Skipped = SkippedItemsString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (Skipped.Count() != 0)
                    {

                        foreach (var item in Skipped)
                        {
                            Int32 Intitem = Convert.ToInt32(item);
                            var stuffToRemove = Mis.SingleOrDefault(s => s.ID == Intitem);
                            if (stuffToRemove != null)
                            {
                                Mis.Remove(stuffToRemove);
                            }
                        }
                    }
                    // Note: This sample contains an additional project, NotificationsExtensions.
                    // NotificationsExtensions exposes an object model for creating notifications, but you can also 
                    // modify the strings directly. See UpdateTileWithImageWithStringManipulation_Click for an example

                    // Create a notification for the Square310x310 tile using one of the available templates for the size.
                    ITileSquare310x310SmallImagesAndTextList05 tileContent = TileContentFactory.CreateTileSquare310x310SmallImagesAndTextList05();
                    tileContent.TextHeading.Text = "Eξαφανίσεις";
                    tileContent.Image1.Src = Mis[0].ImageSrc;
                    tileContent.TextGroup1Field1.Text = Mis[0].FullName;
                    tileContent.TextGroup1Field2.Text = Mis[0].Town;
                    tileContent.Image2.Src = Mis[1].ImageSrc;
                    tileContent.TextGroup2Field1.Text = Mis[1].FullName;
                    tileContent.TextGroup2Field2.Text = Mis[1].Town;
                    tileContent.Image3.Src = Mis[2].ImageSrc;
                    tileContent.TextGroup3Field1.Text = Mis[2].FullName;
                    tileContent.TextGroup3Field2.Text = Mis[2].Town;

                    // Create a notification for the Wide310x150 tile using one of the available templates for the size.
                    ITileWide310x150PeekImageCollection05 wide310x150Content = TileContentFactory.CreateTileWide310x150PeekImageCollection05();
                    wide310x150Content.ImageMain.Src = Mis[0].ImageSrc;
                    wide310x150Content.ImageSecondary.Src = Mis[0].ImageSrc;
                    wide310x150Content.ImageSmallColumn1Row1.Src = Mis[1].ImageSrc;
                    wide310x150Content.ImageSmallColumn1Row2.Src = Mis[2].ImageSrc;
                    wide310x150Content.ImageSmallColumn2Row1.Src = Mis[3].ImageSrc;
                    wide310x150Content.ImageSmallColumn2Row2.Src = Mis[4].ImageSrc;
                    wide310x150Content.TextHeading.Text = Mis[0].Date;
                    wide310x150Content.TextBodyWrap.Text = Mis[0].FullName + " Εξαφανίστηκε από " + Mis[0].Town;

                    // Create a notification for the Square150x150 tile using one of the available templates for the size.
                    ITileSquare150x150PeekImageAndText04 square150x150Content = TileContentFactory.CreateTileSquare150x150PeekImageAndText04();
                    square150x150Content.Image.Src = Mis[0].ImageSrc;
                    square150x150Content.TextBodyWrap.Text = Mis[0].FullName + Mis[0].Town;

                    // Create a notification for the Square71x71 tile using one of the available templates for the size.
                    ITileSquare71x71Image square71x71Content = TileContentFactory.CreateTileSquare71x71Image();
                    square71x71Content.Image.Src = "ms-appx:///Assets/Square70x70Logo.scale-100";
                    square71x71Content.Image.Alt = "Silver Alert";

                    // Attach the Square71x71 template to the Square150x150 template.
                    square150x150Content.Square71x71Content = square71x71Content;

                    // Attach the Square150x150 template to the Wide310x150 template.
                    wide310x150Content.Square150x150Content = square150x150Content;

                    // Attach the Wide310x150 to the Square310x310 template.
                    tileContent.Wide310x150Content = wide310x150Content;

                    // Send the notification to the application’s tile.
                    TileUpdateManager.CreateTileUpdaterForApplication().Update(tileContent.CreateNotification());
                }
            }
            catch (Exception)
            {
            }

        }

    }
}

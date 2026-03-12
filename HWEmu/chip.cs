using Raylib_cs;

namespace HWEmu
{
    public class DrawableChip
    {
        public required Rectangle Rectangle { get; set; }
    }

    public class Chip : Connectable
    {

        public static void DrawChip()
        {
            //Raylib.DrawLineEx(and.IOs[0].Position, and.RightCircleCenter, lineThick, Color.DarkGray);
        }

        public static void CreateAChipInstance()
        {

        }

        public static void FormDrawableChip(Chip chip)
        {
            foreach(IO io in chip.IOs)
            {

            }
        }

        public override void CheckIfInputShouldChange(Connector c)
        {
            throw new NotImplementedException();
        }
    }
}

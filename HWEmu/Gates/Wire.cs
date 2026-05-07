using Raylib_cs;
using System.Numerics;

namespace HWEmu.Gates
{
    public class Wire : Connectable
    {
        private Vector2 OutputPoint { get; set; }
        private Vector2 InputPoint { get; set; }

        const int scale = 1;
        const float lineThick = 10 * scale;

        public Wire(Vector2 rightPoint)
        {
            OutputPoint = rightPoint;
            InputPoint = rightPoint - new Vector2(100, 0);

            Inputs.Add(new Input
            {
                Position = InputPoint,
                State = false,
                Name = "In",
                Guid = Guid.NewGuid(),
                Parent = this,
            });
            Outputs.Add(new Output
            {
                Position = OutputPoint,
                State = false,
                Guid = Guid.NewGuid(),
                Name = "Out",
                Parent = this
            });
        }

        public static void DrawWire(Wire wire)
        {
            if (wire.Inputs[0].State)
            {
                Raylib.DrawLineEx(wire.InputPoint, wire.OutputPoint, lineThick, Color.DarkBlue);
            }
            else
            {
                Raylib.DrawLineEx(wire.InputPoint, wire.OutputPoint, lineThick, Color.DarkGray);
            }

            if (Program.showLinkablePositions)
            {
                DrawConnectionPoints(wire.Inputs, wire.Outputs);
            }
        }

        public override bool UpdateInputIfItShouldChange(Connector connector, bool ChangeInputState = false)
        {
            bool changed = false;

            // Check if state is different
            if (ChangeInputState || Inputs[0].State != connector.State)
            {
                ChangeInputState = true;

                foreach (Connector c in Program.Connectors)
                {
                    // Check if one of the connectors provides true
                    if (c.State == true)
                    {
                        // Just make sure it is not the same connector
                        // So make sure that they have same input, different old output
                        if (c.NewInput.Guid == connector.NewInput.Guid && c.OldOutput != connector.OldOutput)
                        {
                            ChangeInputState = false;
                        }
                    }
                }
                // if false constant true input is provide by another connector
                if (ChangeInputState)
                {
                    // Set the input state
                    Inputs[0].State = connector.State;

                    // Update output
                    Outputs[0].State = Inputs[0].State;

                    // Then update connectors that are related to output
                    foreach (Connector c in Program.Connectors)
                    {
                        if (c.OldOutput.Guid == Outputs[0].Guid)
                        {
                            // Set connector state
                            c.State = Outputs[0].State;
                            Logic.ConnectorStateQueue.Add(c);
                            changed = true;
                        }
                    }
                }
            }
            return changed;
        }
    }
}

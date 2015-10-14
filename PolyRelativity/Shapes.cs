using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDIDrawer;
using System.Drawing;

namespace PolyRelativity
{
  //Instantiate interface that all will use to draw themselves to the canvas
  public interface IRender
  {
    void Render(CDrawer dr);
  }
  //Instantiate interface requiring animated balls have a call that moves them one frame forward
  public interface IAnimate
  {
    void Tick();
  }
  //Basic must abstract class. Implements IRender
  public abstract class Shape:IRender
  {
    protected Point position;
    //Getter for point
    public Point Position
    {
      get { return position; }
    }
    protected Color color;
    protected Shape parent;

    protected int size;
    public int Size
    {
      get { return size; }
    }
    //Basic constructor. All derived shapes must have these parameters.
    public Shape(Point p, Color c,Shape parent=null)
    {
      this.position = p;
      this.color = c;
      this.parent = parent;
    }
    //Placeholder virtual method. Allows subshapes to define their own meaning of render, but still have render universally callable.
    protected abstract void vRender(CDrawer dr);
    public void Render(CDrawer dr)
    {
      if(parent!=null)dr.AddLine(position.X, position.Y, parent.position.X, parent.position.Y, Color.White);
      vRender(dr);
    }


  }
  //Subclass of Shape, implements IAnimate
  public abstract class AniShape : Shape, IAnimate
  {
    protected double sequence_val;
    protected double sequence_delta;
    public AniShape(Point p, Color c,double sv,double sd)
      : base(p, c) //Handle point and color as the basic Shape would
    {
      //Additional animate-only values are assigned from constructor.
      this.sequence_delta = sd;
      this.sequence_val = sv;
    }
    //Create placeholder virtual method
    public abstract void vTick();

    public void Tick()
    {
      vTick();
    }
  }
  //Basic square, uncomplicated. Has additional size parameter, set in constructor.
  class FixedSquare : Shape
  {
    public FixedSquare(Point p,Color c,Shape parent=null)
      : base(p,c,parent)
    {
      this.size = 20;
    }
    //Implements unique version of vRender, drawing a rectangle. .Render() still calls vRender.
    protected override void vRender(CDrawer canvas)
    {
      canvas.AddRectangle(position.X - 10, position.Y - 10, size, size, color);
    }
  }
  //Class for animated polygons. Extends AniShape.
  public class AniPoly : AniShape
  {
    //Uniquely needs additional field: sides
    protected int sides;
    public AniPoly(Point p, Color c,int sides,Shape parent,double sd,double sv=0)
      : base(p, c,sv,sd)
    {
      //Sets rules for what sides can be. Sets size to 25
      if (sides >= 3) this.sides = sides;
      else throw new ArgumentException("Shape must have at least 3 sides");
      this.size = 25;
    }
    protected override void vRender(CDrawer dr)
    {
      dr.AddPolygon(position.X, position.Y, size, sides, sequence_val, color);
    }
    public override void vTick()
    {
      this.sequence_val += this.sequence_delta;
    }
  }
  //Abstract class whose defining trait is not being an orphan
  public abstract class AniChild : AniShape
  {
    protected double distance;
    public AniChild(Point p, Color c, Shape parent,double sd,double sv,double distance)
      : base(p, c,sv,sd)
    {
      this.distance = distance;
      if (parent != null) this.parent = parent;
      else throw new ArgumentException("Must supply a non-null parent");
    }
  }
  //General form of ball with parents.
  public abstract class AniBall : AniChild
  {
    public AniBall(Point p, Color c, Shape parent, double sd, double sv, double distance)
      : base(p, c, parent,sd,sv,distance)
    {
      this.size = 20;
    }
    protected override void vRender(CDrawer dr)
    {
      dr.AddCenteredEllipse(position.X, position.Y, size, size, color);
    }
  }
  //Flashy ball with parents.
  class AniHighlight : AniChild
  {
    private int temp_size;
    private bool grow_direction = true;
    public AniHighlight(Color c, double distance, Shape parent, double sd)
      :base(new Point(0,0),c,parent,sd,0,distance)
    {
      this.size = parent.Size;
      this.temp_size = size;
      this.position = parent.Position;
    }
    public override void vTick()
    {
      if (temp_size + sequence_delta >= distance && grow_direction)
      {
        grow_direction=false;
      }
      else if (!grow_direction && temp_size - sequence_delta < size)
      {
        grow_direction = true;
      }
      temp_size = grow_direction ? temp_size + (int)sequence_delta : temp_size - (int)sequence_delta;


    }
    protected override void vRender(CDrawer dr)
    {
      dr.AddCenteredEllipse(position.X, position.Y, temp_size, temp_size, Color.White);
    }
  }
  //Animated ball ball that orbits its parent
  class OrbitBall : AniBall
  {
    private double ratio;
    public OrbitBall(Color c,double distance, Shape parent, double sd ,double sv=0,double ratio=1)
      : base(new Point(0,0), c, parent, sd, sv, distance)
    {
      this.ratio = ratio;
    }
    public override void vTick()
    {
      this.position.Y = parent.Position.Y+(int)(Math.Sin(sequence_val)*distance*ratio);
      this.position.X = parent.Position.X + (int)(Math.Cos(sequence_val) * distance);
      sequence_val += sequence_delta;
    }
  }
  //Wobbles around its parent on a vertical axis
  class VWobbleBall : AniBall
  {
    public VWobbleBall(Color c, double distance, Shape parent, double sd, double sv = 0)
      : base(new Point(0, 0), c, parent, sd, sv, distance) { }
    public override void vTick()
    {
      position.X = parent.Position.X;
      position.Y = parent.Position.Y + (int)(Math.Cos(sequence_val) *distance);
      sequence_val += sequence_delta;
    }
  }
  //Wobbles around its parent on a horizontal axis
  class HWobbleBall : AniBall
  {
    public HWobbleBall(Color c, double distance, Shape parent, double sd, double sv = 0)
      : base(new Point(0, 0), c, parent, sd, sv, distance) { }
    public override void vTick()
    {
      position.X = parent.Position.X + (int)(Math.Cos(sequence_val) *distance);
      position.Y = parent.Position.Y;
      sequence_val += sequence_delta;
    }
  }
  
}


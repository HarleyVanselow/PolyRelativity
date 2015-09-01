using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDIDrawer;
using System.Drawing;

namespace PolyRelativity
{
  public interface IRender
  {
    void Render(CDrawer dr);
  }
  public interface IAnimate
  {
    void Tick();
  }
  public abstract class Shape:IRender
  {
    protected Point position;
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
    public Shape(Point p, Color c,Shape parent=null)
    {
      this.position = p;
      this.color = c;
      this.parent = parent;
    }
    protected abstract void vRender(CDrawer dr);
    public void Render(CDrawer dr)
    {
      if(parent!=null)dr.AddLine(position.X, position.Y, parent.position.X, parent.position.Y, Color.White);
      vRender(dr);
    }


  }
  public abstract class AniShape : Shape, IAnimate
  {
    protected double sequence_val;
    protected double sequence_delta;
    public AniShape(Point p, Color c,double sv,double sd)
      : base(p, c)
    {
      this.sequence_delta = sd;
      this.sequence_val = sv;
    }
    public abstract void vTick();

    public void Tick()
    {
      vTick();
    }
  }
  class FixedSquare : Shape
  {
    public FixedSquare(Point p,Color c,Shape parent=null)
      : base(p,c,parent)
    {
      this.size = 20;
    }
    protected override void vRender(CDrawer canvas)
    {
      canvas.AddRectangle(position.X - 10, position.Y - 10, size, size, color);
    }
  }
  public class AniPoly : AniShape
  {
    private int sides;
    public AniPoly(Point p, Color c,int sides,Shape parent,double sd,double sv=0)
      : base(p, c,sv,sd)
    {
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
  public abstract class AniChild : AniShape
  {
    protected double distance;
    public AniChild(Point p, Color c, Shape parent,double sd,double sv,double distance)
      : base(p, c,sv,sd)
    {
      this.distance = distance;
      if (parent != null) this.parent = parent;
      else throw new ArgumentException("Must supply a non-null parent");
      Tick();
    }
  }
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
  class VWobbleBall : AniBall
  {
    public VWobbleBall(Color c, double distance, Shape parent, double sd,double sv=0)
      : base(new Point(0, 0), c, parent, sd, sv, distance)
    {
      position.X = parent.Position.X;
    }
    public override void vTick()
    {
      position.Y = parent.Position.Y + (int)(Math.Cos(sequence_val) *distance);
      sequence_val += sequence_delta;
    }
  }
  class HWobbleBall : AniBall
  {
    public HWobbleBall(Color c, double distance, Shape parent, double sd,double sv=0)
      : base(new Point(0, 0), c, parent, sd, sv, distance)
    {
      position.Y = parent.Position.Y;
    }
    public override void vTick()
    {
      position.X = parent.Position.X + (int)(Math.Cos(sequence_val) *distance);
      
      sequence_val += sequence_delta;
    }
  }
  
}


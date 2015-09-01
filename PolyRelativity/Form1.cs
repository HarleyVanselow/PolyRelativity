using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GDIDrawer;
using System.Threading;



namespace PolyRelativity
{
  public partial class Form1 : Form
  {
    public static CDrawer canvas = new CDrawer(1000, 1000);
    public EColor c = EColor.Red;
    public static Random rand = new Random();
    static List<Shape> _shapes = new List<Shape>();
    public Form1()
    {
      InitializeComponent();
      //canvas.Scale = 5;


      // center solid anchors
      _shapes.Add(new FixedSquare(new Point(450, 500), Color.Red, null));
      _shapes.Add(new FixedSquare(new Point(550, 500), Color.Red, _shapes[0]));
      // ccw orbit chain
      {
        List<Shape> local = new List<Shape>();
        local.Add(new OrbitBall(Color.Yellow, 50, _shapes[0], -0.1, Math.PI));
        local.Add(new OrbitBall(Color.Pink, 50, local[0], -0.15, Math.PI));
        local.Add(new OrbitBall(Color.Blue, 50, local[1], -0.2, Math.PI));
        local.Add(new OrbitBall(Color.Green, 50, local[2], -0.25, Math.PI));
        _shapes.AddRange(local);
      }
      // cw orbit chain
      {
        List<Shape> local = new List<Shape>();
        local.Add(new OrbitBall(Color.Yellow, 50, _shapes[1], 0.05));
        local.Add(new OrbitBall(Color.Pink, 50, local[0], 0.075));
        local.Add(new OrbitBall(Color.Blue, 50, local[1], 0.1));
        local.Add(new OrbitBall(Color.Green, 50, local[2], 0.125));
        _shapes.AddRange(local);
      }
      // fixed/double h/v wobble + orbit chain
      {
        List<Shape> local = new List<Shape>();
        local.Add(new FixedSquare(new Point(200, 500), Color.Cyan, null));
        local.Add(new VWobbleBall(Color.Red, 100, local[0], 0.1));
        local.Add(new HWobbleBall(Color.Red, 100, local[1], 0.15));
        local.Add(new OrbitBall(Color.LightBlue, 25, local[2], 0.2));
        _shapes.AddRange(local);
      }
      // show the top row of solid blocks with incremental offset animated vwballs
      {
        List<Shape> localA = new List<Shape>();
        List<Shape> localB = new List<Shape>();
        for (int i = 50; i < 1000; i += 50)
          localA.Add(new FixedSquare(new Point(i, 100), Color.Cyan, null));
        _shapes.AddRange(localA);
        double so = 0;
        foreach (Shape s in localA)
        {
          localB.Add(new VWobbleBall(Color.Purple, 50, s, 0.1, so += 0.7));

        }
        _shapes.AddRange(localB);
      }

      // show 3-tier cloud of quad balls orbiting the same block
      {
        List<Shape> local = new List<Shape>();
        local.Add(new FixedSquare(new Point(800, 500), Color.GreenYellow, null));
        local.Add(new OrbitBall(Color.Yellow, 30, local[0], 0.1, 0));
        local.Add(new OrbitBall(Color.Yellow, 30, local[0], 0.1, Math.PI / 2));
        local.Add(new OrbitBall(Color.Yellow, 30, local[0], 0.1, Math.PI));
        local.Add(new OrbitBall(Color.Yellow, 30, local[0], 0.1, 3 * Math.PI / 2));
        local.Add(new OrbitBall(Color.Yellow, 60, local[0], -0.05, 0));
        local.Add(new OrbitBall(Color.Yellow, 60, local[0], -0.05, Math.PI / 2));
        local.Add(new OrbitBall(Color.Yellow, 60, local[0], -0.05, 3 * Math.PI));
        local.Add(new OrbitBall(Color.Yellow, 60, local[0], -0.05, 3 * Math.PI / 2));
        local.Add(new OrbitBall(Color.Yellow, 90, local[0], 0.025, 0));
        local.Add(new OrbitBall(Color.Yellow, 90, local[0], 0.025, Math.PI / 2));
        local.Add(new OrbitBall(Color.Yellow, 90, local[0], 0.025, Math.PI));
        local.Add(new OrbitBall(Color.Yellow, 90, local[0], 0.025, 3 * Math.PI / 2));
        _shapes.AddRange(local);
      }
      { // show OrbitBall with ratio value, elliptical orbits
        List<Shape> local = new List<Shape>();
        local.Add(new FixedSquare(new Point(500, 400), Color.Violet, null));
        local.Add(new OrbitBall(Color.Yellow, 200, local[0], 0.1, 0, 2));
        local.Add(new OrbitBall(Color.Yellow, 200, local[0], 0.1, Math.PI / 2, 2));
        local.Add(new OrbitBall(Color.Yellow, 200, local[0], 0.1, Math.PI, 2));
        local.Add(new OrbitBall(Color.Yellow, 200, local[0], 0.1, 3 * Math.PI / 2, 2));
        local.Add(new OrbitBall(Color.Yellow, 200, local[0], -0.05, 0, 0.5));
        local.Add(new OrbitBall(Color.Yellow, 200, local[0], -0.05, Math.PI / 2, 0.5));
        local.Add(new OrbitBall(Color.Yellow, 200, local[0], -0.05, 3 * Math.PI, 0.5));
        local.Add(new OrbitBall(Color.Yellow, 200, local[0], -0.05, 3 * Math.PI / 2, 0.5));
        _shapes.AddRange(local);
      }
      // show animated polygons (interlocking triangles)
      _shapes.Add(new AniPoly(new Point(100, 300), Color.Tomato, 3, null, 0.1));
      _shapes.Add(new AniPoly(new Point(135, 300), Color.Tomato, 3, null, -0.1, 1));
      _shapes.Add(new AniPoly(new Point(170, 300), Color.Tomato, 3, null, 0.1));
      // show string of adjacent relative horizontal wobble balls
      {
        List<Shape> local = new List<Shape>();
        local.Add(new FixedSquare(new Point(500, 200), Color.Wheat, null));
        for (int i = 1; i < 20; ++i)
          local.Add(new HWobbleBall(Color.Orange, 25, local[i - 1], 0.1));
        _shapes.AddRange(local);
      }
      // show highlight on a fixed square
      {
        List<Shape> local = new List<Shape>();
        local.Add(new FixedSquare(new Point(800, 300), Color.LightCoral, null));
        local.Add(new AniHighlight(Color.Yellow, 30, local[0], 1));
        _shapes.AddRange(local);
      }



    }
    private void timer1_Tick(object sender, EventArgs e)
    {
      canvas.Clear();
      foreach (Shape s in _shapes)
      {

        if (s is AniShape)
        {
          AniShape ani_s = (AniShape)s;
          ani_s.Tick();
        }
        s.Render(canvas);
      }

      canvas.Render();
    }








  }
}

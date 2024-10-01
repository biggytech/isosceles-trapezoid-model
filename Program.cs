// т.к. в C# не поддерживается множественное наследование классов,
// но поддерживается множественное наследование интерфейсов,
// мы создадим интерфейсы ILocation и IPrimitive
// для дальнейшего наследования классом Point

interface ILocation
{
    public double x { get; set; }
    public double y { get; set; }
}

// класс описания положения
class Location : ILocation
{
    public double x { get; set; }
    public double y { get; set; }

    public Location()
    {
        x = 0;
        y = 0;
    }
    public Location(double x, double y)
    {
        this.x = x;
        this.y = y;
    }
    public Location(Location location)
    {
        x = location.x;
        y = location.y;
    }
}

// класс ограничивающей области
class Clip
{
    public Location start;
    public Location end;

    public Clip()
    {
        start = new Location();
        end = new Location();
    }
    public Clip(Location start, Location end)
    {
        this.start = start; // левая нижняя точка
        this.end = end; // правая верхняя точка
    }
    public Clip(Clip clip)
    {
        start = clip.start;
        end = clip.end;
    }
}

// класс для хранения общих констант и методов проверки различных ограничений на размещение фигур в плоскости
static class Geometry
{
    public static String default_color = "#000000";

    // проверка помещается ли фигура в ограничивающую область
    public static bool does_figure_fits_clip(Clip clip, IsoscelesTrapezoid trapezoid)
    {
        // x0, y0, x1, y1
        (double, double, double, double) region = trapezoid.region;
        return (
            region.Item1 >= clip.start.x &&
            region.Item2 >= clip.start.y &&
            region.Item3 <= clip.end.x &&
            region.Item4 <= clip.end.y
        );
    }

    public static String get_visibility_message(IPrimitive primitive)
    {
        return "Фигура видна?: " + (primitive.is_visible ? "Да" : "Нет") +
            ". Цвет фигуры: " + primitive.color + ".";
    }
}

interface IPrimitive
{
    public String color { get; set; }
    public bool is_visible { get; set; }
}

// класс геометрического примитива
// для хранения и редактирования оформительских свойств фигуры
// В C# нельзя наследоваться от статического класса,
// поэтому класс Primitive просто использует поля/методы из Geometry
class Primitive: IPrimitive
{
    public String color { get; set; } // HEX цвет
    public bool is_visible { get; set; }

    public Primitive()
    {
        color = Geometry.default_color;
        is_visible = true;
    }
    public Primitive(string color, bool is_visible)
    {
        this.color = color;
        this.is_visible = is_visible;
    }
    public Primitive(Primitive primitive)
    {
        color = primitive.color;
        is_visible = primitive.is_visible;
    }

    public String get_visibility_message()
    {
        return Geometry.get_visibility_message(this);
    }
}

// класс точки
class Point : ILocation, IPrimitive {
    // поля, эмулирующие множественное наследование классов
    public Location location;
    public Primitive primitive;

    public double x {
        get { return location.x; }
        set { location.x = value; }
    }
    public double y
    {
        get { return location.y; }
        set { location.y = value; }
    }
    public String color
    {
        get { return primitive.color; }
        set { primitive.color = value; }
    }
    public bool is_visible {
        get { return primitive.is_visible; }
        set { primitive.is_visible = value; }
    }

    public Point()
    {
        location = new Location();
        primitive = new Primitive();
    }
    public Point(Location location, Primitive primitive)
    {
        this.location = location;
        this.primitive = primitive;
    }
    public Point(Point point)
    {
        location = point.location;
        primitive = point.primitive;
    }
}

// интерфейс для отображения и изменения свойств фигуры
interface IIsoscelesTrapezoid
{
    double b { get; set; }
    double c { get; set; }
    double h { get; set; }
    double side { get; }
    double perimeter { get; }
    double area { get; }
    // x0, y0, x1, y1
    (double, double, double, double) region { get; }

}

// класс фигуры - правильная трапеция
class IsoscelesTrapezoid : Point, IIsoscelesTrapezoid
{
    public double b { get; set; } // длина верхнего основания
    public double c { get; set; } // длина нижнего основания
    public double h { get; set; } // высота
    
    // вычисляемые значения
    public double side // боковая сторона
    {
        get
        {
            return Math.Sqrt(Math.Pow((c - b) / 2, 2) + Math.Pow(h, 2));
        }
    }
    public double perimeter // периметр
    {
        get
        {
            return b + c + 2 * side;
        }
    }
    public double area // площадь
    {
        get
        {
            return ((b + c) * h) / 2;
        }
    }
    public (double, double, double, double) region // ограничивающая область
    {
        get
        {
            return (location.x, location.y, location.x + c, location.y + h);
        }
    }

    public IsoscelesTrapezoid(): base()
    {
        b = 30;
        c = 42;
        h = 8;
    }
    public IsoscelesTrapezoid(double b, double c, double h, Location location, Primitive primitive): base(location, primitive)
    {
        this.b = b;
        this.c = c;
        this.h = h;
    }
    public IsoscelesTrapezoid(IsoscelesTrapezoid trapezoid): base(trapezoid.location, trapezoid.primitive)
    {
        b = trapezoid.b;
        c = trapezoid.c;
        h = trapezoid.h;
    }
}

class Program
{
    static void Main(string[] args)
    {
        // создание фигуры
        IsoscelesTrapezoid trapezoid = new IsoscelesTrapezoid();

        Program.ModifyFigure(trapezoid);
    }

    // функция для начала модифицирования фигуры
    public static void ModifyFigure(IsoscelesTrapezoid trapezoid)
    {
        Console.WriteLine("--- Создание ограничивающей области ---");
        Console.WriteLine("Введите ширину ограничивающей области:");
        double width = Convert.ToDouble(Console.ReadLine());
        Console.WriteLine("Введите высоту ограничивающей области:");
        double height = Convert.ToDouble(Console.ReadLine());

        Location start = new Location(0, 0);
        Location end = new Location(width, height);
        Clip clip = new Clip(start, end);

        Console.WriteLine("--- Введение координат левой нижней точки трапеции ---");
        Console.WriteLine("Введите x0:");
        double fig_x0 = Convert.ToDouble(Console.ReadLine());
        Console.WriteLine("Введите y0:");
        double fig_y0 = Convert.ToDouble(Console.ReadLine());

        // установка координат фигуры
        Location trapezoid_start = new Location(fig_x0, fig_y0);
        trapezoid.location = trapezoid_start;

        Console.WriteLine("--- Введение размеров трапеции ---");
        Console.WriteLine("Введите длину c (нижнее основание):");
        double c = Convert.ToDouble(Console.ReadLine());
        Console.WriteLine("Введите длину b (верхнее основание):");
        double b = Convert.ToDouble(Console.ReadLine());
        Console.WriteLine("Введите длину h (высота):");
        double h = Convert.ToDouble(Console.ReadLine());

        trapezoid.c = c;
        trapezoid.b = b;
        trapezoid.h = h;

        (double, double, double, double) region = trapezoid.region;

        Console.WriteLine("--- Выведение вычисленных значений ---");
        Console.WriteLine("Длина боковой стороны: " + Math.Round(trapezoid.side, 2));
        Console.WriteLine("Периметр: " + Math.Round(trapezoid.perimeter, 2));
        Console.WriteLine("Площадь: " + Math.Round(trapezoid.area, 2));
        Console.WriteLine("Ограничивающая область: (" + region.Item1 + ", " + region.Item2 + "), (" + region.Item3 + ", " + region.Item4 + ")");
        Console.WriteLine("Трапеция помещается в область?: " +
                (Geometry.does_figure_fits_clip(clip, trapezoid) ? "Да" : "Нет"));
    }
}
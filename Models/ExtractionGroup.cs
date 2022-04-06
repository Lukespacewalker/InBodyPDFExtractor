using System.Collections.Generic;

namespace InBodyPDFExtractor.Models;

internal abstract class ExtractionGroup
{
    public string PropertyName { get; set; } = string.Empty;
    //public Type DataType { get; set; }
    public List<IdBound> Bounds = new List<IdBound>();
    public abstract object GetData();

    public ExtractionGroup(string propertyName, IEnumerable<IdBound> bounds)
    {
        PropertyName = propertyName;
        Bounds.AddRange(bounds);
    }

    public ExtractionGroup(string propertyName, IdBound bound)
    {
        PropertyName = propertyName;
        Bounds.Add(bound);
    }
}

internal class ExtractionGroup<T> : ExtractionGroup
{
    public ExtractionGroup(string propertyName, T data, IEnumerable<IdBound> bounds) : base(propertyName, bounds)
    {
        Data = data;
    }

    public ExtractionGroup(string propertyName, T data, IdBound bound) : base(propertyName, bound)
    {
        Data = data;
    }

    public T Data { get; set; }

    public override object GetData() => (object)Data!;
}

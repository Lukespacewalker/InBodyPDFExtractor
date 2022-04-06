using Syncfusion.Pdf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using InBodyPDFExtractor.Extensions;

namespace InBodyPDFExtractor.Models;

internal enum Gender
{
    Male, Female, Other
}
internal class Person
{
    public string Name { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public double Age { get; set; } = 0;
    public double Height { get; set; } = 0;
    public DateTime DateTime { get; set; } = DateTime.MinValue;
    public Gender Gender { get; set; } = Gender.Other;

    public double Weight { get; set; } = 0;
    public double SkeletalMuscleMass { get; set; } = 0;
    public double BodyFatMass { get; set; } = 0;
    public double TotalBodyWater { get; set; } = 0;
    public double BasalMetabolicRatio { get; set; } = 0;

    private static Dictionary<string, Action<Person, object>> setters = new Dictionary<string, Action<Person, object>>();

    public static Person CreateFromExtractionGroup(IEnumerable<ExtractionGroup> extractionGroups)
    {
        Person person = new();
        foreach (var extractionGroup in extractionGroups)
        {
            var setter = setters[extractionGroup.PropertyName];
            setter(person, extractionGroup.GetData());
        }
        return person;
    }

    static Person()
    {
        CreateSetters();
    }
    private static void CreateSetters()
    {
        var propertyInfos = typeof(Person).GetProperties();
        foreach (var propertyInfo in propertyInfos)
        {
            var valParam = Expression.Parameter(typeof(object));
            var objParam = Expression.Parameter(typeof(Person));
            var body = Expression.Call(objParam, propertyInfo.SetMethod!, Expression.Convert(valParam, propertyInfo.PropertyType));
            Expression<Action<Person, object>> lambda = Expression.Lambda<Action<Person, object>>(body, objParam, valParam);
            setters.Add(propertyInfo.Name, lambda.Compile());
        }
    }
}

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

internal record DataWithNormalRange(double Value,double? Lower,double? Upper);

internal class Person
{
    public string Name { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public double Age { get; set; } = 0;
    public double Height { get; set; } = 0;
    public DataWithNormalRange? Weight { get; set; }
    public DateTime DateTime { get; set; } = DateTime.MinValue;
    public Gender Gender { get; set; } = Gender.Other;
    // Body Composition
    public DataWithNormalRange? SkeletalMuscleMass { get; set; }
    public DataWithNormalRange? BodyFatMass { get; set; }
    public DataWithNormalRange? TotalBodyWater { get; set; }
    public DataWithNormalRange? FatFreeMass { get; set; }
    // Obesity Diagnosis
    public DataWithNormalRange? BodyMassIndex { get; set; }
    public DataWithNormalRange? PercentBodyFatIndex { get; set; }
    public DataWithNormalRange? WaistHipRatioIndex { get; set; }
    public DataWithNormalRange? BasalMetabolicRatioIndex { get; set; }

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

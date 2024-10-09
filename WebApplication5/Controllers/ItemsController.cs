using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApplication5.DTO;

[ApiController]
[Route("api/[controller]")]

public class ItemsController : ControllerBase
{
    private readonly IMongoCollection<Certificate> _items;

    public ItemsController(IMongoClient client)
    {
        var database = client.GetDatabase("awsweld");
        _items = database.GetCollection<Certificate>("Certificate");
    }



    [HttpGet("all")]
    public ActionResult<List<Certificate>> Getall()
    {
        return _items.Find(item => true).ToList();
    }


    [HttpPost]
    public ActionResult<Certificate> Create(Certificate certificate)
    {
       
       
            _items.InsertOne(certificate);
        return Ok(certificate);
    }
    [HttpPut("{id}")]
    public IActionResult Update(string id, Certificate updatedCertificate)
    {
        var objectId = new ObjectId(id);

       
        var result = _items.ReplaceOne(item => item.Id == objectId, updatedCertificate);

        if (result.MatchedCount == 0)
        {
            return NotFound(new { message = "Certificate not found" });
        }

        return Ok(updatedCertificate);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var objectId = new ObjectId(id);

        
        var result = _items.DeleteOne(item => item.Id == objectId);

        if (result.DeletedCount == 0)
        {
            return NotFound(new { message = "Certificate not found" });
        }

        return Ok(new { message = "Certificate deleted successfully" });
    }

    [HttpGet("inspector/{inspectorNumber:int}")]
    public ActionResult<Certificate> GetByInspectorNumber(int inspectorNumber)
    {
        var certificate = _items.Find(item => item.InspectorNumber == inspectorNumber).FirstOrDefault();

        if (certificate == null)
        {
            return NotFound(new { message = "Certificate not found with the given InspectorNumber" });
        }

        return Ok(certificate);
    }

}

public class Certificate
{
    public ObjectId Id { get; set; }
    public int InspectorNumber { get; set; }
    public int Roll_number { get; set; }
    public string InspectorName { get; set; }
    public string FatherName { get; set; }
    public bool Radiographic_testing_level_II { get; set; } = default;
    public bool Ultrasonic_testing_level_II { get; set; } = default;
    public bool Visual_testing_level_II { get; set; } = default;
    public bool Liquid_penetrant_testing_level_II { get; set; } = default;
    public bool Magnetic_particle_testing_level_II { get; set; } = default;
    


}
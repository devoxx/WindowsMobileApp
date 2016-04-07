using MyDevoxx.Model;
using MyDevoxx.Services.RestModel;
using System.Collections.Generic;

namespace MyDevoxx.Converter.Model
{
    public class FloorConverter
    {
        public static List<Floor> apply(CFP cfp)
        {
            List<Floor> floorList = new List<Floor>();
            foreach (Services.Floor f in cfp.floors)
            {
                Floor floor = new Floor();
                floor.confId = cfp.id;
                floor.img = f.img;
                floor.tabpos = f.tabpos;
                floor.target = f.target;
                floor.title = f.title;
                floorList.Add(floor);
            }

            return floorList;
        }
    }
}
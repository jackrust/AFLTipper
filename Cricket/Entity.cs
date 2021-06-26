using MongoDB.Bson;

namespace Cricket
{
    public class Entity
    {
        public ObjectId Id { get; set; }
        public Entity()
        {
            //Id = ObjectId.GenerateNewId();
        }
    }
}

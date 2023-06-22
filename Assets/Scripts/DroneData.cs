namespace Drone
{
    public class DroneData
    {
        private float _health;
        private float _armor;
        private float _battery;
        private int _coins;
        private int _crystals;

        public int Crystals
        {
            get => _crystals;
            set => _crystals = value;
        }
        
        public int Coins
        {
            get => _coins;
            set => _coins = value;
        }
        
        public float Health
        {
            get => _health;
            set
            {
                if (value > 100)
                    _health = 100;
                else if (_health < 0)
                    _health = 0;
                else
                    _health = value;
            }
        }

        public float Armor
        {
            get => _armor;
            set
            {
                if (value > 100)
                    _armor = 100;
                else if (value < 0)
                    _armor = 0;
                else
                    _armor = value;
            }
        }
        
        public float Battery
        {
            get => _battery;
            set
            {
                if (value > 100)
                    _battery = 100;
                else if (value < 0)
                    _battery = 0;
                else
                    _battery = value;
            }
        }

        public DroneData(float health, float armor, float battery)
        {
            _health = health;
            _armor = armor;
            _battery = battery;
        }
    }
}
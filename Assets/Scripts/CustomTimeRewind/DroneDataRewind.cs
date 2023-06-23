using System;
using Drone;

public class DroneDataRewind : RewindAbstract
{
    public DroneRpgController droneRpgController;
    private CircularBuffer<float> _trackedHealth;
    private CircularBuffer<float> _trackedArmor;
    private CircularBuffer<float> _trackedBattery;

    private void Start()
    {
        _trackedHealth = new CircularBuffer<float>();
        _trackedArmor = new CircularBuffer<float>();
        _trackedBattery = new CircularBuffer<float>();
    }

    public override void Track()
    {
        TrackDroneData();
    }
    
    public override void Rewind(float seconds)
    {
        RestoreDroneData(seconds);
    }
    
    private void TrackDroneData()
    {
        _trackedHealth.WriteLastValue(droneRpgController.DroneData.Health);
        _trackedArmor.WriteLastValue(droneRpgController.DroneData.Armor);
        _trackedBattery.WriteLastValue(droneRpgController.DroneData.Battery);
    }

    private void RestoreDroneData(float seconds)
    {
        droneRpgController.DroneData.Health = _trackedHealth.ReadFromBuffer(seconds);
        droneRpgController.DroneData.Armor = _trackedArmor.ReadFromBuffer(seconds);
        droneRpgController.DroneData.Battery = _trackedBattery.ReadFromBuffer(seconds);
    }
}
# Behavior_Tree
 
Demonstrates Millington's behavior tree as diagrammed on pg. 344. Uses an event-driven tree rather than recursion in order to provide timing features.

See:
* EventBus.cs - modified from Baron Ch. 15 to provide delayed event triggering
* Arriver.cs - modified from DynamicSteering project to provide an OnArrived event
* Task.cs - implements all needed tasks
* BruceBanner.cs - builds and executes the behavior tree


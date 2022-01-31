using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;


public class ParkingAgent : Agent {

    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material failMaterial;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    public Text resultText;

    public override void OnEpisodeBegin() {
        transform.localPosition =  new Vector3(Random.Range(-8f,+7f),-1.225409f,1.93f);
      //  transform.localPosition =  new Vector3(Random.Range(-6.8f,+6.8f),-0.6114092f,Random.Range(-4.6f,+3.52f));
        targetTransform.localPosition =  new Vector3(Random.Range(-8f,+7f),-1.225409f,-11.967f);
    }

   public override void OnActionReceived(ActionBuffers actions) {
       float moverX = actions.ContinuousActions[0];
       float moverZ = actions.ContinuousActions[1];
       float velocidad = 12f;
       transform.localPosition += new Vector3(moverX,0,moverZ) *Time.deltaTime * velocidad;
       //  floorMeshRenderer.material = defaultMaterial;

       //Debug.Log(actions.ContinuousActions[0]);
   }

   public override void CollectObservations(VectorSensor sensor) {
       sensor.AddObservation(transform.localPosition);
       sensor.AddObservation(targetTransform.localPosition);
   }

   private void OnTriggerEnter(Collider other) {
       if (other.TryGetComponent<Parking>(out Parking parking))
       {
       //Debug.Log("da recompenza");
        resultText.text = "It has complete the attempt succesfully";
        floorMeshRenderer.material = winMaterial;
        SetReward(+1f);
        EndEpisode();

       }
       if (other.TryGetComponent<Pared>(out Pared pared))
       {
        floorMeshRenderer.material = failMaterial;
        resultText.text = "It has failed the attempt";

        SetReward(-1f);
        EndEpisode();
       }

       if (other.TryGetComponent<Conos>(out Conos conos))
       {
         floorMeshRenderer.material = failMaterial;
         resultText.text = "It has failed the attempt";

         SetReward(-0.25f);
         EndEpisode();
       }
   }

   public override void Heuristic(in ActionBuffers actionsOut) {
       ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
       continuousActions[0] = Input.GetAxisRaw("Horizontal");
       continuousActions[1] = Input.GetAxisRaw("Vertical");
   }

   private void Update() {
     if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
   }
}

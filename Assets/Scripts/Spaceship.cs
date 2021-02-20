using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour{

    public GameObject[] Planets;
    public int speed;

    private Vector3 velocity = new Vector3(0.01f, 0, 0);
    private Vector3 force;

    public float mass = 420E+3f;
    private float G = 6.67E-11f;

    // Start is called before the first frame update
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){

        for (int j = 0; j < speed; j++) {

            force = new Vector3(0, 0, 0);

            for (int i = 0; i < Planets.Length; i++) {
                float deltaX = Planets[i].transform.position.x - this.transform.position.x;
                float deltaY = Planets[i].transform.position.y - this.transform.position.y;
                float norme = Mathf.Pow(Mathf.Pow(deltaX, 2) + Mathf.Pow(deltaY, 2), 0.5f);
                Vector3 Force_unit = new Vector3(deltaX / norme, deltaY / norme, 0);

                force = force + (G * Planets[i].GetComponent<Planet>().mass * mass / Mathf.Pow(1000000 * norme, 2)) / 1000000 * Force_unit;

            }   

            velocity = velocity + force * Time.deltaTime / mass;
            move(velocity);
        }
    }

    void move(Vector3 velocity) {

        this.transform.position = this.transform.position + (velocity*Time.deltaTime);

	}
}

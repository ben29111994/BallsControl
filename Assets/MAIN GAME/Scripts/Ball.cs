using UnityEngine;

public class Ball : MonoBehaviour
{
    public ColorSet activeColors;
    public Color inactiveColor;
    public bool isPreactive;
    public GameObject breakEffect;

    Material mat;
    Rigidbody myRB;

    bool isActive = false;
    public bool IsActive
    {
        get
        {
            return isActive;
        }
        set
        {
            isActive = value;

            if (isActive)
            {
                mat.color = activeColors.GetColor;
                myRB.isKinematic = false;
            }
            else
            {
                mat.color = inactiveColor;
                myRB.isKinematic = true;
            }
        }
    }

    private void OnEnable()
    {
        myRB = GetComponent<Rigidbody>();
        mat = GetComponent<MeshRenderer>().material;
        IsActive = isPreactive;

        // transform.localScale = Vector3.one * Random.Range(0.1f, 0.2f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            // Ball otherBall = collision.gameObject.GetComponent<Ball>();

            // if (otherBall != null)
            //     if (otherBall.IsActive)
            //     {
            //         if (!isActive)
            //         {
            //             IsActive = true;
            //         }
            //     }
        }

        // if (collision.relativeVelocity.magnitude > 8)
        // {
        //     SoundManager.Instance.Hit(collision.relativeVelocity.magnitude);
        // }
    }

    [System.Serializable]
    public class ColorSet
    {
        public Color[] colors;

        public Color GetColor
        {
            get
            {
                if (colors != null)
                    return colors[Random.Range(0, colors.Length)];
                else
                    return Color.black;
            }
        }
    }

    public void PrepareToDestroy()
    {
        myRB.isKinematic = false;
    }

    private void OnTriggerEnter(Collider other) {
        if((other.CompareTag("Ball") && name != other.name))
        {
            var effect = Instantiate(breakEffect, other.transform.position, Quaternion.identity);
            effect.GetComponent<ParticleSystem>().startColor = mat.color;
            GameController.instance.RemoveBall(other.gameObject);
            Destroy(other.gameObject);
        }

        if(other.CompareTag("Obstacle"))
        {
            var effect = Instantiate(breakEffect, transform.position, Quaternion.identity);
            effect.GetComponent<ParticleSystem>().startColor = mat.color;
            GameController.instance.RemoveBall(gameObject);
            Destroy(gameObject);
        }
    }
}

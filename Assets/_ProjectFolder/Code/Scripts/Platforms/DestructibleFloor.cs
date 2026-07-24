using System.Collections;
using UnityEngine;

public class PlataformaRompibleConFeedback : MonoBehaviour
{
    [Header("Tiempos de Estado")]
    [Tooltip("Tiempo total en segundos que la plataforma aguanta antes de desaparecer.")]
    [SerializeField] private float tiempoParaDesaparecer = 1.5f;

    [Tooltip("Tiempo que la plataforma permanece invisible antes de volver.")]
    [SerializeField] private float tiempoParaReaparecer = 3.0f;

    [Header("Configuración del Parpadeo (Feedback)")]
    [Range(0f, 1f)]
    [Tooltip("El momento (porcentaje) dentro del 'tiempoParaDesaparecer' en que empieza a parpadear. 0.5f = a la mitad del tiempo.")]
    [SerializeField] private float momentoInicioParpadeo = 0.5f; // Comienza al 50% del tiempo

    [Tooltip("Cuántas veces parpadea por segundo al final.")]
    [SerializeField] private float velocidadParpadeoFinal = 15f;

    [Header("Referencias (Asignar automáticamente)")]
    [SerializeField] private Collider2D colisionador;
    [SerializeField] private SpriteRenderer renderizador;

    private bool rompiendose = false;
    private Color colorOriginal;

    private void Awake()
    {
        if (colisionador == null) colisionador = GetComponent<Collider2D>();
        if (renderizador == null) renderizador = GetComponent<SpriteRenderer>();

        if (renderizador != null)
        {
            colorOriginal = renderizador.color;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !rompiendose)
        {
            // Verificación simplificada de pisada por arriba
            if (collision.contacts[0].normal.y < -0.5f)
            {
                StartCoroutine(RutinaRomperYReconstruir());
            }
        }
    }

    private IEnumerator RutinaRomperYReconstruir()
    {
        rompiendose = true;

        // --- FASE 1: Espera inicial tranquila ---
        float tiempoTranquilo = tiempoParaDesaparecer * momentoInicioParpadeo;
        yield return new WaitForSeconds(tiempoTranquilo);

        // --- FASE 2: Parpadeo de advertencia ---
        float tiempoDeParpadeoTotal = tiempoParaDesaparecer * (1f - momentoInicioParpadeo);
        float tiempoPasado = 0f;

        while (tiempoPasado < tiempoDeParpadeoTotal)
        {
            // Aumentamos la velocidad del parpadeo conforme se acaba el tiempo
            float progresoDeParpadeo = tiempoPasado / tiempoDeParpadeoTotal;
            float velocidadActual = Mathf.Lerp(velocidadParpadeoFinal / 3f, velocidadParpadeoFinal, progresoDeParpadeo);

            // Calculamos la opacidad (alfa) usando un seno para un efecto suave, o alternamos drásticamente
            // Aquí usamos alternancia drástica (encendido/apagado rápido) para mayor urgencia
            float frecuencia = velocidadActual * Mathf.PI * 2f;
            float alfa = (Mathf.Sin(tiempoPasado * frecuencia) > 0) ? 1f : 0.1f; // Alterna entre 100% y 10% de opacidad

            // Aplicamos el nuevo color con el alfa modificado
            renderizador.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, alfa);

            tiempoPasado += Time.deltaTime;
            yield return null; // Espera al siguiente frame
        }

        // Aseguramos que el color vuelva a la normalidad antes de desactivar (por si acaso)
        renderizador.color = colorOriginal;

        // --- FASE 3: Desaparecer ---
        colisionador.enabled = false;
        renderizador.enabled = false;

        // --- FASE 4: Espera de reaparición ---
        yield return new WaitForSeconds(tiempoParaReaparecer);

        // --- FASE 5: Reconstruir ---
        renderizador.enabled = true;
        colisionador.enabled = true;

        rompiendose = false;
    }
}
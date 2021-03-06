# 1 "c:\\Users\\Joel\\Documents\\Arduino\\generated_examples\\MQTTClient\\ALS_MQTTClient\\Device\\device.ino"
# 1 "c:\\Users\\Joel\\Documents\\Arduino\\generated_examples\\MQTTClient\\ALS_MQTTClient\\Device\\device.ino"
# 2 "c:\\Users\\Joel\\Documents\\Arduino\\generated_examples\\MQTTClient\\ALS_MQTTClient\\Device\\device.ino" 2
# 3 "c:\\Users\\Joel\\Documents\\Arduino\\generated_examples\\MQTTClient\\ALS_MQTTClient\\Device\\device.ino" 2
# 4 "c:\\Users\\Joel\\Documents\\Arduino\\generated_examples\\MQTTClient\\ALS_MQTTClient\\Device\\device.ino" 2
# 5 "c:\\Users\\Joel\\Documents\\Arduino\\generated_examples\\MQTTClient\\ALS_MQTTClient\\Device\\device.ino" 2



int status = WL_IDLE_STATUS;
bool hasWifi = false;

const char *mqttServer = "test.mosquitto.org";
const int mqttPort = 1883;

const char *mqttDeviceId = "ALSIoTDevkit_8a6fc70e2f1d40a68379a9b439b7346d";
const char *mqttUsername = "";
const char *mqttPassword = "";

enum LEDS
{
  LED_A,
  LED_B
};

void InitWiFi()
{
  Screen.print(2, "Connecting...");

  if (WiFi.begin() == WL_CONNECTED)
  {
    IPAddress ip = WiFi.localIP();
    Screen.print(1, ip.get_address());
    hasWifi = true;
    Screen.print(2, "Running... \r\n");
    Screen.print(3, "");
  }
  else
  {
    Screen.print(1, "No Wi-Fi\r\n ");
  }
}

void ToggleLed(LEDS led)
{
  const char *topic = "ToggleLed";

  char messageBuffer[2];
  switch (led)
  {
  case LED_A:
    sprintf(messageBuffer, "A");
    break;

  case LED_B:
    sprintf(messageBuffer, "B");
    break;

  default:
    return;
  }

  Screen.print(2, "Sending...");

  MQTTNetwork mqttNetwork;
  MQTT::Client<MQTTNetwork, Countdown> client = MQTT::Client<MQTTNetwork, Countdown>(mqttNetwork);

  int rc = mqttNetwork.connect(mqttServer, mqttPort);
  if (rc != 0)
  {
    Screen.print(2, "Failed");
    Serial.println("Connected to MQTT server failed");
  }

  MQTTPacket_connectData data = { {'M', 'Q', 'T', 'C'}, 0, 4, {
# 73 "c:\\Users\\Joel\\Documents\\Arduino\\generated_examples\\MQTTClient\\ALS_MQTTClient\\Device\\device.ino" 3 4
                               __null
# 73 "c:\\Users\\Joel\\Documents\\Arduino\\generated_examples\\MQTTClient\\ALS_MQTTClient\\Device\\device.ino"
                               , {0, 
# 73 "c:\\Users\\Joel\\Documents\\Arduino\\generated_examples\\MQTTClient\\ALS_MQTTClient\\Device\\device.ino" 3 4
                               __null
# 73 "c:\\Users\\Joel\\Documents\\Arduino\\generated_examples\\MQTTClient\\ALS_MQTTClient\\Device\\device.ino"
                               }}, 60, 1, 0, { {'M', 'Q', 'T', 'W'}, 0, {
# 73 "c:\\Users\\Joel\\Documents\\Arduino\\generated_examples\\MQTTClient\\ALS_MQTTClient\\Device\\device.ino" 3 4
                               __null
# 73 "c:\\Users\\Joel\\Documents\\Arduino\\generated_examples\\MQTTClient\\ALS_MQTTClient\\Device\\device.ino"
                               , {0, 
# 73 "c:\\Users\\Joel\\Documents\\Arduino\\generated_examples\\MQTTClient\\ALS_MQTTClient\\Device\\device.ino" 3 4
                               __null
# 73 "c:\\Users\\Joel\\Documents\\Arduino\\generated_examples\\MQTTClient\\ALS_MQTTClient\\Device\\device.ino"
                               }}, {
# 73 "c:\\Users\\Joel\\Documents\\Arduino\\generated_examples\\MQTTClient\\ALS_MQTTClient\\Device\\device.ino" 3 4
                               __null
# 73 "c:\\Users\\Joel\\Documents\\Arduino\\generated_examples\\MQTTClient\\ALS_MQTTClient\\Device\\device.ino"
                               , {0, 
# 73 "c:\\Users\\Joel\\Documents\\Arduino\\generated_examples\\MQTTClient\\ALS_MQTTClient\\Device\\device.ino" 3 4
                               __null
# 73 "c:\\Users\\Joel\\Documents\\Arduino\\generated_examples\\MQTTClient\\ALS_MQTTClient\\Device\\device.ino"
                               }}, 0, 0 }, {
# 73 "c:\\Users\\Joel\\Documents\\Arduino\\generated_examples\\MQTTClient\\ALS_MQTTClient\\Device\\device.ino" 3 4
                               __null
# 73 "c:\\Users\\Joel\\Documents\\Arduino\\generated_examples\\MQTTClient\\ALS_MQTTClient\\Device\\device.ino"
                               , {0, 
# 73 "c:\\Users\\Joel\\Documents\\Arduino\\generated_examples\\MQTTClient\\ALS_MQTTClient\\Device\\device.ino" 3 4
                               __null
# 73 "c:\\Users\\Joel\\Documents\\Arduino\\generated_examples\\MQTTClient\\ALS_MQTTClient\\Device\\device.ino"
                               }}, {
# 73 "c:\\Users\\Joel\\Documents\\Arduino\\generated_examples\\MQTTClient\\ALS_MQTTClient\\Device\\device.ino" 3 4
                               __null
# 73 "c:\\Users\\Joel\\Documents\\Arduino\\generated_examples\\MQTTClient\\ALS_MQTTClient\\Device\\device.ino"
                               , {0, 
# 73 "c:\\Users\\Joel\\Documents\\Arduino\\generated_examples\\MQTTClient\\ALS_MQTTClient\\Device\\device.ino" 3 4
                               __null
# 73 "c:\\Users\\Joel\\Documents\\Arduino\\generated_examples\\MQTTClient\\ALS_MQTTClient\\Device\\device.ino"
                               }} };
  data.MQTTVersion = 4;

  sprintf(data.clientID.cstring, mqttDeviceId);
  sprintf(data.username.cstring, mqttUsername);
  sprintf(data.password.cstring, mqttPassword);

  if ((rc = client.connect(data)) != 0)
  {
    Screen.print(2, "Failed");
    Serial.println("MQTT client connect to server failed");
  }

  MQTT::Message message;
  message.qos = MQTT::QOS0;
  message.retained = false;
  message.dup = false;
  message.payload = (void *)messageBuffer;
  message.payloadlen = strlen(messageBuffer) + 1;
  rc = client.publish(topic, message);

  if (rc == 0)
  {
    Screen.print(2, "Success");
  }
  else
  {
    char status[20];
    sprintf(status, "Failed: %d", rc);
    Screen.print(2, (char *)status);
  }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////
// Actions
static void DoIdle()
{
  bool sent = false;
  if (digitalRead(USER_BUTTON_A) == 0x0)
  {
    Screen.print(1, "Toggle A Led\n");
    ToggleLed(LED_A);
    sent = true;
  }
  else if (digitalRead(USER_BUTTON_B) == 0x0)
  {
    Screen.print(1, "Toggle B Led\n");
    ToggleLed(LED_B);
    sent = true;
  }

  if (sent)
  {
    delay(2000);
    Wait();
  }
}

static void Wait()
{
  Screen.print(1, "Press any key");
  Screen.print(2, "Waiting...");
  Screen.print(3, "");
}

void setup()
{
  Screen.init();
  Screen.print(0, "MQTT Client");

  Screen.print(2, "Initializing...");
  pinMode(USER_BUTTON_A, 0x1);
  pinMode(USER_BUTTON_B, 0x1);

  Screen.print(3, " > Serial");
  Serial.begin(115200);

  // Initialize the WiFi module
  Screen.print(3, " > WiFi");
  hasWifi = false;
  InitWiFi();
  if (!hasWifi)
  {
    return;
  }

  Wait();
}

void loop()
{
  if (hasWifi)
  {
    DoIdle();
  }
  delay(100);
}

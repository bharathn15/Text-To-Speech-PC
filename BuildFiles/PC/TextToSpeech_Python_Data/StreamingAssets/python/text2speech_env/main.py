import pyttsx3
import sys

class App:
    
    def __init__(self, engine):
        self.engine = engine
        
    def TextToSpeech(self, inp):
        self.engine.say(inp)
        self.engine.runAndWait()
        print('Speak is invoked.....')

if __name__ == '__main__':
    if(len(sys.argv) > 1):
        txt_inp = ' '.join(sys.argv[1:])
    else:
        txt_inp = 'no input is provided'
    engine = pyttsx3.init()
    app = App(engine)
    app.TextToSpeech(txt_inp)
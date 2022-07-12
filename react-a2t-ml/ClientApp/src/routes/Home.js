import axios from "axios";
import React, { useEffect, useState } from "react";
import { ImSpinner8 } from 'react-icons/im';
import { FaPlay, FaPause, FaSpinner, FaDotCircle } from 'react-icons/fa';
import SpeechRecognition, { useSpeechRecognition } from 'react-speech-recognition';
import { motion, AnimatePresence, useMotionValue } from "framer-motion";



const Home = (props) => {
    const {
        transcript,
        interimTranscript,
        finalTranscript,
        listening,
        resetTranscript,
        browserSupportsSpeechRecognition,
        isMicrophoneAvailable
    } = useSpeechRecognition();

    const formattingOption = {
        style: 'percent',
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    }

    const formatter = new Intl.NumberFormat("en-US", formattingOption);
    

    const [recordedTranscript, setRecordedTranscript] = useState(null);
    const [prediction, setPrediction] = useState(null);
    const [saving, setSaving] = useState(false);
    //const [recentPrediction, setRecentPrediction] = useState(null);

    const predict = async (text) => {
        const json = JSON.stringify({ Text: text });

        let res = await axios.post("http://localhost:5001/api/sentiments/", json, { headers: { 'Content-Type': 'application/json' } });
        console.log('res: ', res)
        setPrediction(res.data);
        console.log('res.data', res.data)



        return res.data;
    }

    useEffect(() => {
        console.log('Recorded Transcript: ', recordedTranscript)
        if (recordedTranscript) {
            predict(recordedTranscript)
        }
    }, [recordedTranscript])

    useEffect(() => {
        console.log("Microphone available?: ", isMicrophoneAvailable);
    }, [])

    useEffect(() => {
        console.log("Microphone listening?: ", listening)
    }, [listening])

    useEffect(() => {
        console.log("Prediction: ", prediction)
    }, [prediction])

    //useEffect(() => {
    //    //
    //}, [showPrediction])

    

    if (!browserSupportsSpeechRecognition) {
        return <span>Browser doesn't support speech recognition.</span>;
    }

    

    return (
        <div className="flex justify-center w-full flex-col items-center">
            
            <div className="max-w-7xl px-32 py-28 bg-white m-12 flex flex-col items-center justify-center rounded overflow-hidden shadow-lg">
                <div className="px-6 py-4">
                    <div className="font-bold text-3xl mb-2 text-center">Audio to Text Sentiment</div>
                    <p className="text-gray-700 text-base max-w-lg m-4">
                        This is a quick example of using a react-speech-recognition hook (a wrapper for the <a href="https://developer.mozilla.org/en-US/docs/Web/API/SpeechRecognition">Web Speech API</a>)
                        to convert audio input to text. Sentiments are saved to a local db because why not?
                    </p>
                    <p className="text-gray-700 text-base max-w-lg m-4">
                        Data is trained from a twitter dataset using tensorflow. The model is given to MLNet on the backend for inference when calling the /sentiments API (POST).
                    </p>
                </div>
                <p className="font-bold text-xl mb-2 text-center">Microphone: <span className={!listening ? "text-red-500" : "text-green-500"}>{listening ? 'ON' : 'OFF'}</span></p>
                <p className="font-bold text-xl mb-2 text-center" >Microphone available? <span className={!isMicrophoneAvailable ? "text-red-500" : "text-green-500"}>{isMicrophoneAvailable ? "TRUE" : "FALSE"}</span></p>
                <motion.button
                    whileHover={{
                        scale: 0.8,
                        transition: { duration: 0.2 },
                    }}
                    whileTap={{
                        scale: 0.9,
                        transition: { duration: 1 },
                    }}
                    className="w-32 h-32 rounded-full text-white my-16"
                    
                    onClick={ async(e) => {
                        //console.log('here')
                        e.preventDefault();
                        if (!listening) {
                            //console.log('trying to listen?')
                            setRecordedTranscript(null);
                            setPrediction(null);
                            await SpeechRecognition.startListening({ continuous: true });
                        }
                        else {
                            await SpeechRecognition.stopListening();
                            console.log('transcript: ', transcript);
                            setRecordedTranscript(transcript);
                            resetTranscript();
                        }
                    }}
                >
                    <AnimatePresence>
                        <motion.div
                            layout
                            key="modal"
                            initial={{ opacity: 0 }}
                            animate={{ opacity: 1 }}
                            exit={{ opacity: 0 }}
                            transition={{ duration: 2, delay: 0 }}
                        >
                            {listening ?
                                <FaPause className="mx-auto text-red-500 w-32 h-32 " />
                                :
                                <FaPlay className="mx-auto text-emerald-500 w-32 h-32 " />
                            }
                        </motion.div>
                    </AnimatePresence>
                </motion.button>

                {/* TRANSCRIPT */}
                <AnimatePresence>
                    {transcript && transcript != "" ?
                        <motion.div
                            key="modal"
                            initial={{ opacity: 0 }}
                            animate={{ opacity: 1 }}
                            exit={{ opacity: 0 }}
                            className="mt-2 max-w-2xl max-h-16">
                            <h2 className="text-black font-bold text-2xl m-2 text-center">Transcript</h2>
                            <div className="flex flex-col h-auto shadow-sm rounded justify-center items-center bg-gray-100 p-4 mt-2 max-w-72">
                                <p className="text-black font-bold text-center ">{transcript}</p>
                            </div>
                        </motion.div>
                        : null}
                </AnimatePresence>

                {/* RESULT */}
                <AnimatePresence>
                    {prediction && prediction.data ?
                        <motion.div
                            key="modal"
                            initial={{ opacity: 0 }}
                            animate={{ opacity: 1 }}
                            exit={{ opacity: 0 }}
                            className="max-w-2xl max-h-16 mb-16 mt-4">
                            <h2 className="text-black font-bold text-2xl m-2 text-center">Results</h2>
                            <div className="flex flex-col h-auto shadow-sm rounded justify-center items-center bg-gray-100 p-4 mt-2 max-w-72">
                                <p className="text-black font-bold text-center ">
                                    {"Negative Sentiment: " + formatter.format(prediction.data[0][0])}
                                    {/*{prediction.data[0].toString()}*/}
                                </p>
                                <p className="text-black font-bold text-center ">
                                    {"Positive Sentiment: " + formatter.format(prediction.data[0][1])}
                                    {/*{prediction.data[0].toString()}*/}
                                </p>
                                <p className="text-black font-bold text-center  text-xl mt-2">
                                    {prediction.data[0][1] > prediction.data[0][0] ? <span className="text-green-600">Positive</span> : <span className="text-red-600">Negative</span>}
                                </p>
                            </div>
                        </motion.div>
                        : null}
                </AnimatePresence>

                {/*<AnimatePresence>*/}
                {/*    <motion.div*/}
                {/*        className="mt-32 flex flex-col justify-center items-center"*/}
                {/*    >*/}
                {/*        <motion.button*/}
                {/*            animate={{ opacity: 1 }}*/}
                {/*            exit={{ opacity: 0 }}*/}

                {/*            className="bg-green-500 hover:bg-green-600 text-white font-bold py-2 px-4 rounded w-48 h-16"*/}
                {/*            onClick={() => {*/}
                {/*                setSaving(!saving);*/}
                                
                {/*            }}*/}
                {/*        >*/}
                {/*            <span className="text-xl">*/}
                {/*                Save Prediction*/}
                {/*            </span>*/}
                {/*        </motion.button>*/}
                {/*    </motion.div>*/}



                {/*    {!saving ?*/}
                {/*        <motion.div*/}
                {/*            className="mt-24"*/}
                {/*            animate={{ opacity: 1, rotate: 360 }}*/}
                {/*            transition={{*/}
                {/*                repeat: Infinity,*/}
                {/*                duration: 0.2,*/}
                {/*                //repeatType: "mirror",*/}
                {/*                delay: 0,*/}
                {/*                type: "just"*/}
                {/*            }}*/}
                {/*            exit={{ opacity: 0 }}*/}
                {/*        >*/}
                {/*            <ImSpinner8 className="text-xl mx-auto text-green-500" />*/}
                {/*        </motion.div>*/}
                {/*        : null}*/}

                {/*</AnimatePresence>*/}
                
                
                {/*<p>{prediction ? prediction : null}</p>*/}
            </div>
            
        </div>
       
    )
}

export default Home;
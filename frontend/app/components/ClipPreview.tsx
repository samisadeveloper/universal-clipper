import type { Clip } from "~/ClipLoader";

import { MdCloudUpload } from "react-icons/md";
import { FaDiscord } from "react-icons/fa";

import { BiSolidInvader } from "react-icons/bi";

import { FaVolumeMute } from "react-icons/fa";
import { FaVolumeUp } from "react-icons/fa";

import { useState, type MouseEventHandler, type ReactNode } from "react";
import { motion } from "motion/react";

const primaryColor = "#2a2d40";

const secondaryColor = "#362973";

const secondaryText = '#b6b6b8';

function Button({icon, text, clicked}: {icon: ReactNode, text: string, clicked: MouseEventHandler<HTMLButtonElement>}) {
        return (
                <motion.button 
                whileHover={{scale: 1.1}}
                whileTap={{ scale: 0.95 }}

                onClick={clicked} style={{backgroundColor: secondaryColor, width: 200, borderRadius: 5, justifyContent: 'center', display: 'flex', gap: 5, alignItems: 'center'}}>
                        {icon}
                        <p style={{fontSize: 20}}>{text}</p>
                </motion.button>
        );
}

export default function ClipPreview({clip}: {clip: Clip}) {
        const [muted, setMuted] = useState(true);

        return (
                <div style={{ backgroundColor: primaryColor, boxShadow: '5px 5px 25px rgba(0, 0, 0, 0.85)', borderRadius: 5, margin: 15 }}>
                        <motion.div 
                        onMouseEnter={(event) => {
                                const video = event.currentTarget.querySelector('video') as HTMLVideoElement;
                                video.play();
                        }} 

                        onMouseLeave={(event) => {
                                const video = event.currentTarget.querySelector('video') as HTMLVideoElement;
                                video.pause();
                                video.currentTime = 0;

                                setMuted(true);
                        }}

                        onClick={() => {
                                setMuted(!muted);
                        }}

                        whileHover="zoomed"

                        style={{ position: 'relative', overflow: "hidden", width: '800', height: '800'}}>
                                <motion.video 
                                        variants={{zoomed: {scale: 1.05}}}
                                        muted={muted}
                                        playsInline
                                        loop
                                        width="800"
                                        height="800"
                                        src={`http://localhost:5000/media/${clip.id}.mp4`}
                                />

                                <motion.div 
                                        style={{ 
                                                backgroundColor: primaryColor,
                                                borderRadius: 10,
                                                justifyItems: 'center',
                                                alignContent: 'center',
                                                width: 35,
                                                height: 35,
                                                zIndex: 10,
                                                top: 16,
                                                right: 16,
                                                position: 'absolute'
                                }}>
                                        {muted && <FaVolumeMute style={{color: 'white', fontSize: 20}} />}
                                        {!muted && <FaVolumeUp style={{color: 'white', fontSize: 20}} />}
                                </motion.div>
                        </motion.div>
                        <div style={{marginLeft: 15, marginTop: 5}}>
                                <h1 style={{ fontWeight: '530', fontSize: 25}}>{clip.title}</h1>

                                <div style={{marginRight: 10, marginLeft: 10, justifyContent: 'space-between', display: 'flex', alignItems: 'center'}}>
                                        <div style={{display: 'flex', gap: 10, alignItems: 'center'}}>
                                                <BiSolidInvader style={{color: secondaryText}} />
                                                <p style={{color: secondaryText, fontSize: 20}}>Unknown</p>
                                        </div>
                                        <p style={{color: secondaryText}}>3 Oct 2026</p>
                                </div>

                                <div style={{ marginTop: 25, marginBottom: 15, display: 'flex', justifyContent: 'space-around', gap: 20, height: '200' }}>
                                        <Button icon={<MdCloudUpload style={{fontSize:20}} />} text="Upload and share" clicked={() => {
                                                console.log("hello sir");
                                        }}/>

                                        <Button icon={<FaDiscord style={{fontSize: 20}} /> } text="Share to Discord" clicked={() => {
                                                console.log("hello sir");
                                        }}/>
                                </div>
                        </div>
                </div>
        );
}

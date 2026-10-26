import type { Clip } from "~/ClipLoader";

import { BiSolidInvader } from "react-icons/bi";

import { motion } from "motion/react";
import { useEffect, useRef, useState, type RefObject } from "react";
import MutedButton from "./MutedButton";
import ExitButton from "./ExitButton";

const primaryColor = "#2a2d40";

const secondaryText = '#b6b6b8';

function Timeline({videoRef}: {videoRef: RefObject<HTMLVideoElement | null>}) {
        const [progress, setProgress] = useState(0);
        const animationRef = useRef<number | null>(null);

        useEffect(() => {
                const updateProgressLoop = () => {
                        const video = videoRef.current;

                        if (video && !video.paused && video.duration) {
                                setProgress(video.currentTime / video.duration);
                        }

                        animationRef.current = requestAnimationFrame(updateProgressLoop);
                };

                animationRef.current = requestAnimationFrame(updateProgressLoop);

                return () => {
                        if (animationRef.current) {
                                cancelAnimationFrame(animationRef.current);
                        }
                };
        }, [videoRef]);

        return (
                <div style={{width: '100%', marginTop: 25, height: 100}}>
                        <motion.div 
                                drag="x"

                                onClick={() => {
                                        videoRef.current?.pause();
                                }}
                                onDrag={(event) => {
                                        // somehow turn this back into progress?
                                }}

                                whileDrag={{scale: 1.1}}

                                dragMomentum={false}

                                style={{width: '20px', height: '100%', backgroundColor: 'black', borderRadius: 15, position: 'relative', left: `${progress * 100}%`}} 
                        />
                </div>
        );
}

export default function EditableClip({clip}: {clip: Clip}) {
        const [muted, setMuted] = useState(true);

        const videoRef = useRef<HTMLVideoElement & { current: HTMLVideoElement | null }>(null) as any; 

        return (
                <div style={{ backgroundColor: primaryColor, boxShadow: '5px 5px 25px rgba(0, 0, 0, 0.85)', borderRadius: 5, margin: 15, width: '95%' }}>
                        <motion.div 

                        style={{ position: 'relative', overflow: "hidden"}}>
                                <motion.video 
                                        muted={muted}
                                        playsInline
                                        autoPlay
                                        loop
                                        width='100%'
                                        height='100%'
                                        style={{objectFit: 'cover'}}
                                        src={`http://localhost:5000/media/${clip.id}.mp4`}
                                        ref={videoRef}
                                />

                                <MutedButton muted={muted} setMuted={setMuted} color={primaryColor} />
                                <ExitButton color={primaryColor} />

                        </motion.div>

                        <div style={{marginLeft: 15, marginTop: 5}}>
                                <h1 style={{ fontWeight: '530', fontSize: 25}}>{clip.title}</h1>

                                <div style={{marginRight: 10, marginLeft: 10, justifyContent: 'space-between', display: 'flex', alignItems: 'center'}}>
                                        <div style={{display: 'flex', gap: 10, alignItems: 'center'}}>
                                                <BiSolidInvader style={{color: secondaryText}} />
                                                <p style={{color: secondaryText, fontSize: 20}}>Unknown</p>
                                        </div>
                                </div>
                        </div>

                        <Timeline videoRef={videoRef}/>
                </div>
        );
}

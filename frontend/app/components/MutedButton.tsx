import { motion } from "motion/react";
import { FaVolumeMute, FaVolumeUp } from "react-icons/fa";

export default function MutedButton({color, setMuted, muted}: {color: string, setMuted: Function, muted: boolean}) {
        return (
                <motion.div 

                        onClick={() => {
                                setMuted(!muted);
                        }}

                        style={{ 
                                backgroundColor: color,
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
        );
}

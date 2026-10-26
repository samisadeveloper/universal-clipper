import { motion } from "motion/react";
import { GoX } from "react-icons/go";

export default function ExitButton({color}: {color: string}) {
        return (
                <motion.div 

                        onClick={() => {
                                // modalRef.current?.
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
                                left: 16,
                                position: 'absolute'
                        }}>

                        <GoX style={{color: 'white', fontSize: 20}} />
                </motion.div>
        );
}
